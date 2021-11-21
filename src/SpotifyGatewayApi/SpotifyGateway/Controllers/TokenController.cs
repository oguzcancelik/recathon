using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyGateway.Infrastructure.Attributes;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Resources.Abstractions;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.LogServices.Abstractions;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Tokens)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly IAuthSettings _authSettings;
        private readonly IErrorResources _errorResources;
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _loggerService;

        public TokenController(
            IAuthSettings authSettings,
            IErrorResources errorResources,
            ITokenService tokenService,
            ILoggerService loggerService
        )
        {
            _authSettings = authSettings;
            _errorResources = errorResources;
            _tokenService = tokenService;
            _loggerService = loggerService;
        }

        [HttpGet(RouteConstants.Generate)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(UserAuthenticationAttribute))]
        public async Task<IActionResult> Get()
        {
            var sessionGuid = Request.GetAuthToken(_authSettings.SessionKey);

            var response = new BaseResponse<TokenResponse>();

            if (string.IsNullOrEmpty(sessionGuid))
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(AuthExtensions.GetCookie), Request.GetCookie(AuthConstants.AuthToken) },
                    { nameof(AuthExtensions.GetHeader), Request.GetHeader(AuthConstants.AuthToken) }
                };

                await _loggerService.LogWarningAsync(_errorResources.UserNotFound, nameof(TokenController), nameof(Get), logValues);

                response.Errors.Add(new CustomError(_errorResources.UserNotFound));

                return BadRequest(response);
            }

            response = await _tokenService.GenerateTokenAsync();

            return Ok(response);
        }
    }
}