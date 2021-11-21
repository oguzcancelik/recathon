using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyGateway.Infrastructure.Attributes;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Playlists)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class PlaylistController : BaseController
    {
        private readonly IAuthSettings _authSettings;
        private readonly IBrowseService _browseService;
        private readonly IPlaylistService _playlistService;
        private readonly ITokenService _tokenService;

        public PlaylistController(
            IAuthSettings authSettings,
            IBrowseService browseService,
            IPlaylistService playlistService,
            ITokenService tokenService
        )
        {
            _authSettings = authSettings;
            _browseService = browseService;
            _playlistService = playlistService;
            _tokenService = tokenService;
        }

        [HttpGet(RouteConstants.Browse)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryResponse>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Browse()
        {
            var response = await _browseService.GetCategoryPlaylistsAsync();

            if (!response.HasError && response.Result != null)
            {
                return Ok(response);
            }

            if (!response.HasError && response.Result == null)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        [HttpPost(RouteConstants.Recommend)]
        [ProducesResponseType(typeof(BaseResponse<GeneratedPlaylistResponse>), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(UserAuthenticationAttribute))]
        [ServiceFilter(typeof(AppAuthenticationAttribute))]
        public async Task<IActionResult> Post([FromBody] GeneratePlaylistRequest request)
        {
            var response = new BaseResponse<GeneratedPlaylistResponse>();

            if (!Request.IsAdmin(_authSettings.AdminToken))
            {
                var isDecrypted = await _tokenService.DecryptTokenAsync(request.EncryptedToken);

                if (!isDecrypted)
                {
                    response.Errors.Add(new CustomError(ErrorConstants.InvalidRequest));

                    return BadRequest(response);
                }
            }

            response = await _playlistService.GeneratePlaylistAsync(request);

            if (!response.HasError && response.Result != null)
            {
                return Ok(response);
            }

            if (!response.HasError && response.Result == null)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
    }
}