using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyGateway.Infrastructure.Attributes;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Users)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRecommendationHistoryService _recommendationHistoryService;
        private readonly IUserService _userService;

        public UserController(
            IRecommendationHistoryService recommendationHistoryService,
            IUserService userService
        )
        {
            _recommendationHistoryService = recommendationHistoryService;
            _userService = userService;
        }

        [HttpGet(RouteConstants.PlaylistsAction)]
        [ProducesResponseType(typeof(BaseResponse<List<PlaylistResponse>>), (int) HttpStatusCode.OK)]
        [ServiceFilter(typeof(UserAuthenticationAttribute))]
        public async Task<IActionResult> GetCurrentUsersPlaylists()
        {
            var response = await _userService.GetCurrentUsersPlaylistsAsync();

            if (!response.HasError && response.Result != null && response.Result.Count != 0)
            {
                response.Result = response.Result.Where(x => !x.Name.StartsWith(ApplicationConstants.AppPrefix)).ToList();

                return Ok(response);
            }

            if (!response.HasError && response.Result is {Count: 0})
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        [HttpGet(RouteConstants.History)]
        [ProducesResponseType(typeof(BaseResponse<List<GeneratedPlaylistResponse>>), (int) HttpStatusCode.OK)]
        [ServiceFilter(typeof(UserAuthenticationAttribute))]
        public async Task<IActionResult> GetCurrentUsersRecommendedPlaylists()
        {
            var response = await _recommendationHistoryService.GetUsersRecommendationHistoryAsync();

            if (!response.HasError && response.Result != null && response.Result.Count != 0)
            {
                return Ok(response);
            }

            if (!response.HasError && (response.Result == null || response.Result.Count == 0))
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        [HttpGet(RouteConstants.Token)]
        [ProducesResponseType(typeof(BaseResponse<List<GeneratedPlaylistResponse>>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserToken([FromQuery] string userId)
        {
            var token = await _userService.GetUserTokenAsync(userId);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(token);
            }

            return NotFound();
        }
    }
}