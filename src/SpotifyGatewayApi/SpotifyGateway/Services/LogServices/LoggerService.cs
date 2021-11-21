using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyAPI.Web.Models;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;
using LogLevel = SpotifyGateway.Models.Enums.LogLevel;

namespace SpotifyGateway.Services.LogServices
{
    public class LoggerService : ILoggerService
    {
        private readonly HttpContext _httpContext;
        private readonly IMessageServiceClient _messageServiceClient;
        private readonly IRepository _repository;
        private readonly IGeneralSettings _generalSettings;
        private readonly ITelegramSettings _telegramSettings;
        private readonly ILoggerRepository _loggerRepository;

        public LoggerService(
            IHttpContextAccessor httpContextAccessor,
            IMessageServiceClient messageServiceClient,
            IRepository repository,
            IGeneralSettings generalSettings,
            ITelegramSettings telegramSettings,
            ILoggerRepository loggerRepository
        )
        {
            _httpContext = httpContextAccessor.HttpContext;
            _messageServiceClient = messageServiceClient;
            _repository = repository;
            _generalSettings = generalSettings;
            _telegramSettings = telegramSettings;
            _loggerRepository = loggerRepository;
        }

        public async Task LogErrorAsync(Token token, string className, string methodName, Dictionary<string, object> logValues, int? code)
        {
            await LogErrorAsync(token.ToCustomError(), className, methodName, logValues);
        }

        public async Task LogErrorAsync(Error error, string className, string methodName, Dictionary<string, object> logValues)
        {
            await LogErrorAsync(error.ToCustomError(), className, methodName, logValues);
        }

        public async Task LogErrorAsync(List<CustomError> errors, string className, string methodName, Dictionary<string, object> logValues, string stackTrace = null)
        {
            foreach (var error in errors)
            {
                await LogErrorAsync(error, className, methodName, logValues, stackTrace);
            }
        }

        public async Task LogErrorAsync(string errorMessage, string className, string methodName, Dictionary<string, object> logValues, string stackTrace = null, int? code = null)
        {
            await LogErrorAsync(errorMessage.ToCustomError(code), className, methodName, logValues, stackTrace);
        }

        public async Task LogErrorAsync(CustomError error, string className, string methodName, Dictionary<string, object> logValues, string stackTrace = null)
        {
            var isClientIdDeleted = await IsClientIdDeletedAsync(error.Message, logValues, error.Code);

            var log = new Log
            {
                ClassName = className,
                Code = error.Code,
                ErrorMessage = error.Message,
                LogLevel = LogLevel.Error,
                MethodName = methodName,
                StackTrace = stackTrace,
                UserFriendlyMessage = error.UserFriendlyMessage,
                Values = logValues,
                RequestId = GetRequestId()
            };

            if (_generalSettings.LogLevel >= LogLevel.Error)
            {
                try
                {
                    await _loggerRepository.InsertAsync(log);
                }
                catch (Exception)
                {
                    await _messageServiceClient.SendMessageAsync(log, MessageType.ErrorLog);
                }
            }

            switch (_telegramSettings.LogLevel)
            {
                case TelegramLogLevel.All:
                case TelegramLogLevel.Exception when isClientIdDeleted || !string.IsNullOrEmpty(stackTrace):
                case TelegramLogLevel.Auth when isClientIdDeleted:
                    log.Values = logValues?.RemoveSensitiveValues();
                    await _messageServiceClient.SendMessageAsync(log, MessageType.ErrorLog);
                    break;
                case TelegramLogLevel.None:
                    break;
            }
        }

        public async Task LogWarningAsync(List<CustomError> errors, string className, string methodName, Dictionary<string, object> logValues)
        {
            foreach (var error in errors)
            {
                await LogWarningAsync(error, className, methodName, logValues);
            }
        }

        public async Task LogWarningAsync(string errorMessage, string className, string methodName, Dictionary<string, object> logValues)
        {
            await LogWarningAsync(errorMessage.ToCustomError(), className, methodName, logValues);
        }

        public async Task LogWarningAsync(CustomError error, string className, string methodName, Dictionary<string, object> logValues)
        {
            if (_generalSettings.LogLevel < LogLevel.Warning)
            {
                return;
            }

            var log = new Log
            {
                ClassName = className,
                Code = error.Code,
                ErrorMessage = error.Message,
                LogLevel = LogLevel.Warning,
                MethodName = methodName,
                UserFriendlyMessage = error.UserFriendlyMessage,
                Values = logValues,
                RequestId = GetRequestId()
            };

            try
            {
                await _loggerRepository.InsertAsync(log);
            }
            catch (Exception)
            {
                await _messageServiceClient.SendMessageAsync(log, MessageType.ErrorLog);
            }
        }

        private async Task<bool> IsClientIdDeletedAsync(string errorMessage, IReadOnlyDictionary<string, object> logValues, int? code = null)
        {
            object clientId = null;

            var isDelete = code == (int)HttpStatusCode.Unauthorized &&
                           errorMessage.Contains("invalid") &&
                           (errorMessage.Contains("client") || errorMessage.Contains("access") || errorMessage.Contains("token")) &&
                           logValues.TryGetValue(nameof(clientId), out clientId);

            if (isDelete && !string.IsNullOrEmpty((string)clientId))
            {
                var parameters = new { ClientId = clientId, UpdateTime = DateTime.UtcNow };

                await _repository.ExecuteAsync(QueryConstants.UpdateDeletedCrendentialQuery, parameters);

                return true;
            }

            return false;
        }

        private string GetRequestId()
        {
            return _httpContext?.Items != null && _httpContext.Items.TryGetValue(HttpConstants.RequestId, out var requestId)
                ? (string)requestId
                : default;
        }
    }
}