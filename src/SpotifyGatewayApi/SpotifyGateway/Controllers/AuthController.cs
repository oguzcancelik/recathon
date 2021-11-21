using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Models.Responses.Server.Auth;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Auth)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly GeneralOptions _generalOptions;

        public AuthController(
            IAuthService authService,
            IOptions<GeneralOptions> generalOptions
        )
        {
            _authService = authService;
            _generalOptions = generalOptions.Value;
        }

        [HttpGet("login")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login()
        {
            var authUrl = await _authService.GetAuthUrlAsync();

            return Ok(authUrl);
        }

        [HttpGet(RouteConstants.Info)]
        [ProducesResponseType(typeof(BaseResponse<AuthInfoResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Info()
        {
            var authInfo = await _authService.GetAuthInfoAsync();

            return Ok(authInfo);
        }

        [HttpGet("callback")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Callback([FromQuery] SpotifyAuthorizationRequest request)
        {
            request.DeviceType = DeviceType.Browser;

            var response = await _authService.AuthenticateUserAsync(request);

            if (!response.HasError && response.Result != null)
            {
                Response.SetSessionToken(response.Result);

                return Redirect(_generalOptions.BaseUrl);
            }

            return RedirectToAction("Login");
        }

        [HttpGet(RouteConstants.Callback)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> MobileCallback([FromQuery] SpotifyAuthorizationRequest request)
        {
            request.DeviceType = DeviceType.Mobile;

            var response = await _authService.AuthenticateUserAsync(request);

            if (!response.HasError && response.Result != null)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }
    }
}