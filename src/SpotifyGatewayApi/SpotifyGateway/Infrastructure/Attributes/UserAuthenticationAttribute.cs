using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Resources.Abstractions;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserAuthenticationAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IAuthSettings _authSettings;
        private readonly IErrorResources _errorResources;
        private readonly ISpotifyUserContext _spotifyUserContext;
        private readonly IUserService _userService;
        private readonly ILoggerService _loggerService;

        private readonly ObjectResult _objectResult;

        public UserAuthenticationAttribute(
            IAuthSettings authSettings,
            IErrorResources errorResources,
            ISpotifyUserContext spotifyUserContext,
            IUserService userService,
            ILoggerService loggerService
        )
        {
            _authSettings = authSettings;
            _errorResources = errorResources;
            _spotifyUserContext = spotifyUserContext;
            _userService = userService;
            _loggerService = loggerService;

            _objectResult = new ObjectResult(
                new BaseResponse<object>
                {
                    Errors = new List<CustomError>
                    {
                        new(_errorResources.UserNotFound)
                    }
                }
            )
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasError;
            ObjectResult response;

            string sessionGuid;
            (hasError, sessionGuid, response) = await ControlSessionGuidAsync(context.HttpContext.Request);

            if (hasError)
            {
                context.Result = response;

                return;
            }

            string userId;
            (hasError, userId, response) = await ControlUserIdAsync(sessionGuid);

            if (hasError)
            {
                context.Result = response;

                return;
            }

            _spotifyUserContext.Set(sessionGuid, userId);

            if (RouteConstants.SpotifyAuthenticateRoutes.Any(context.HttpContext.Request.Path.ToString().EqualsIgnoreCase))
            {
                User user;
                (hasError, user, response) = await ControlAuthenticatedUserAsync(userId);

                if (hasError)
                {
                    context.Result = response;

                    return;
                }

                _spotifyUserContext.SetUser(user);
            }

            await next();
        }

        private async Task<(bool HasError, string SessionGuid, ObjectResult Response)> ControlSessionGuidAsync(HttpRequest request)
        {
            var sessionGuid = request.GetAuthToken(_authSettings.SessionKey);

            if (string.IsNullOrEmpty(sessionGuid))
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(AuthExtensions.GetCookie), request.GetCookie(AuthConstants.AuthToken) },
                    { nameof(AuthExtensions.GetHeader), request.GetHeader(AuthConstants.AuthToken) }
                };

                await _loggerService.LogWarningAsync(_errorResources.UserNotFound, nameof(UserAuthenticationAttribute), nameof(ControlSessionGuidAsync), logValues);

                return (true, null, _objectResult);
            }

            return (false, sessionGuid, null);
        }

        private async Task<(bool HasError, string UserId, ObjectResult Response)> ControlUserIdAsync(string sessionGuid)
        {
            var userId = await _userService.GetUserIdBySessionGuidAsync(sessionGuid);

            if (string.IsNullOrEmpty(userId))
            {
                return (true, null, _objectResult);
            }

            return (false, userId, null);
        }

        private async Task<(bool HasError, User user, ObjectResult Response)> ControlAuthenticatedUserAsync(string userId)
        {
            var user = await _userService.GetAuthenticatedUserAsync(userId);

            if (user == null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(userId), userId }
                };

                await _loggerService.LogWarningAsync(_errorResources.UserNotFound, nameof(UserAuthenticationAttribute), nameof(ControlAuthenticatedUserAsync), logValues);

                return (true, null, _objectResult);
            }

            return (false, user, null);
        }
    }
}