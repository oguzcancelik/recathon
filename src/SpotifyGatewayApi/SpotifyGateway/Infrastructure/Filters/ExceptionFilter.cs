using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILoggerService _loggerService;

        public ExceptionFilter(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var response = new BaseResponse<object>();

            var statusCode = (int) HttpStatusCode.BadRequest;

            if (context.Exception.GetType() == typeof(CustomException))
            {
                var exception = (CustomException) context.Exception;

                await _loggerService.LogErrorAsync(
                    exception.Errors, exception.ClassName ?? nameof(ExceptionFilter),
                    exception.MethodName ?? nameof(OnExceptionAsync),
                    exception.LogValues,
                    exception.StackTrace
                );

                response.Errors = exception.Errors;

                var unauthorized = response.Errors.Any(x => x.Code == (int) HttpStatusCode.Unauthorized);
                var logoutUser = unauthorized &&
                                 exception.LogValues.TryGetValue(nameof(CredentialType), out var credentialType) &&
                                 (string) credentialType == CredentialType.Auth.ToString();

                if (logoutUser)
                {
                    statusCode = (int) HttpStatusCode.Unauthorized;

                    response.Errors = new List<CustomError>
                    {
                        new(ErrorConstants.UserAuthenticationError)
                    };
                }
            }
            else
            {
                response.Errors = new List<CustomError>
                {
                    new(ErrorConstants.UnexpectedError)
                };

                await _loggerService.LogErrorAsync(context.Exception.Message, nameof(ExceptionFilter), nameof(OnExceptionAsync), stackTrace: context.Exception.StackTrace);
            }

            context.Result = new ObjectResult(response);
            context.HttpContext.Response.StatusCode = statusCode;
            context.ExceptionHandled = true;
        }
    }
}