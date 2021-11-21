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
    [Route(RouteConstants.Resources)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class ResourceController : BaseController
    {
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IResourceFactory _resourceFactory;

        public ResourceController(
            IRedisCacheProvider redisCacheProvider,
            IResourceFactory resourceFactory
        )
        {
            _redisCacheProvider = redisCacheProvider;
            _resourceFactory = resourceFactory;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var resourcesList = _resourceFactory.GetResources(ResourcesClass.All);

            return Ok(resourcesList.ToJson());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateResourcesRequest request)
        {
            await _redisCacheProvider.PubAsync(RedisConstants.UpdateResourcesEvent, request);

            return NoContent();
        }
    }
}