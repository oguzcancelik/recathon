using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Requests;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Settings)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class SettingController : BaseController
    {
        private readonly ISettingFactory _settingFactory;
        private readonly IRedisCacheProvider _redisCacheProvider;

        public SettingController(
            IRedisCacheProvider redisCacheProvider,
            ISettingFactory settingFactory
        )
        {
            _settingFactory = settingFactory;
            _redisCacheProvider = redisCacheProvider;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var settingsList = _settingFactory.GetSettings(SettingsClass.All);

            var response = settingsList.ToDictionary(x => x.GetType().Name, x => x.GetValues());

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateSettingsRequest request)
        {
            await _redisCacheProvider.PubAsync(RedisConstants.UpdateSettingsEvent, request);

            return NoContent();
        }
    }
}