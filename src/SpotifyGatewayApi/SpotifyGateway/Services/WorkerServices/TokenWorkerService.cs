using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Polly.Retry;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.MessageModels;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Services.WorkerServices
{
    public class TokenWorkerService : ITokenWorkerService
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IMessageServiceClient _messageServiceClient;
        private readonly IRepository _repository;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly ITelegramSettings _telegramSettings;
        private readonly ILoggerService _loggerService;

        public TokenWorkerService(
            AsyncRetryPolicy retryPolicy,
            IMessageServiceClient messageServiceClient,
            IRepository repository,
            IRedisCacheProvider redisCacheProvider,
            ITelegramSettings telegramSettings,
            ILoggerService loggerService
        )
        {
            _retryPolicy = retryPolicy;
            _messageServiceClient = messageServiceClient;
            _repository = repository;
            _redisCacheProvider = redisCacheProvider;
            _telegramSettings = telegramSettings;
            _loggerService = loggerService;
        }

        public async Task RunAsync()
        {
            var tokenWorkerMessage = new TokenWorkerMessage
            {
                ClassName = nameof(TokenWorkerService),
                Subject = MessageSubjectType.TokenWorkerProcess
            };

            var shouldWorkerRun = WorkerHelpers.ShouldWorkerRun(WorkerType.Token);

            tokenWorkerMessage.ShouldWorkerRun = shouldWorkerRun;

            if (!shouldWorkerRun)
            {
                var lastUpdated = await _redisCacheProvider.GetAsync<DateTime?>(RedisConstants.TokenRefreshTimeCache);

                tokenWorkerMessage.IsTokenRefreshTimeExceeded = !lastUpdated.HasValue || (DateTime.UtcNow - lastUpdated.Value).TotalMinutes > WorkerConstants.TokenRefreshTime;
                shouldWorkerRun = tokenWorkerMessage.IsTokenRefreshTimeExceeded.Value;
            }

            if (shouldWorkerRun)
            {
                tokenWorkerMessage.IsSucceed = await RefreshCredentialsAsync();
            }

            var (updateResult, updateCount) = await LoadUpdatedCredentialsAsync();

            tokenWorkerMessage.UpdatedCredentialsResult = updateResult;
            tokenWorkerMessage.UpdatedCredentialsCount = updateCount;

            if (_telegramSettings.IsTokenWorkerMessagesEnabled)
            {
                await _messageServiceClient.SendMessageAsync(tokenWorkerMessage);
            }
        }

        private async Task<bool> RefreshCredentialsAsync()
        {
            try
            {
                var credentials = await _repository.QueryAsync<Credential>(QueryConstants.GetCrendentialsQuery);

                credentials = credentials.GroupBy(x => x.ClientId).Select(x => x.First()).ToList();

                await GetTokensAsync(credentials);

                await _redisCacheProvider.SetAsync(RedisConstants.TokenRefreshTimeCache, DateTime.UtcNow);

                return true;
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, nameof(TokenWorkerService), nameof(RefreshCredentialsAsync), stackTrace: e.StackTrace);

                return false;
            }
        }

        private async Task<(bool?, int?)> LoadUpdatedCredentialsAsync()
        {
            try
            {
                var credentials = await _repository.QueryAsync<Credential>(QueryConstants.GetCrendentialsToUpdateQuery);

                credentials = credentials.GroupBy(x => x.ClientId).Select(x => x.First()).ToList();

                if (credentials.Any())
                {
                    await GetTokensAsync(credentials, true);

                    return (true, credentials.Count);
                }

                return (null, null);
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, nameof(TokenWorkerService), nameof(LoadUpdatedCredentialsAsync), stackTrace: e.StackTrace);

                return (false, null);
            }
        }

        private async Task GetTokensAsync(IEnumerable<Credential> credentials, bool activateCredential = false)
        {
            foreach (var credential in credentials)
            {
                try
                {
                    var token = await _retryPolicy.ExecuteAsync(() => TokenHelpers.GetTokenAsync(credential.ClientId, credential.ClientSecret));

                    if (!token.HasError())
                    {
                        var parameters = new
                        {
                            credential.ClientId,
                            token.AccessToken,
                            IsActive = activateCredential || credential.IsActive,
                            TokenUpdateTime = DateTime.UtcNow,
                            UpdateTime = DateTime.UtcNow
                        };

                        await _repository.ExecuteAsync(QueryConstants.UpdateCredentialQuery, parameters);
                    }
                    else
                    {
                        var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { nameof(credential.ClientId), credential.ClientId },
                            { nameof(SpotifyEndpoint), SpotifyEndpoint.GetToken },
                            { nameof(token), token.ToJson() }
                        };

                        await _loggerService.LogErrorAsync(token, nameof(TokenWorkerService), nameof(GetTokensAsync), logValues, (int)HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(credential.ClientId), credential.ClientId },
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.GetToken }
                    };

                    await _loggerService.LogErrorAsync(e.Message, nameof(TokenWorkerService), nameof(GetTokensAsync), logValues, code: (int)HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}