using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.Requests;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Controllers
{
    [Route(RouteConstants.Workers)]
    [Produces(MimeTypes.JsonType)]
    [ApiController]
    public class WorkerController : BaseController
    {
        private readonly IWorkerManager _workerManager;

        public WorkerController(
            IWorkerManager workerManager
        )
        {
            _workerManager = workerManager;
        }

        [HttpPost(RouteConstants.Start)]
        public async Task<IActionResult> Start([FromQuery] StartWorkerRequest request)
        {
            var result = await _workerManager.StartWorkerAsync(request.WorkerType);

            return Ok(result);
        }

        [HttpPost(RouteConstants.Stop)]
        public async Task<IActionResult> Stop([FromQuery] StopWorkerRequest request)
        {
            var result = await _workerManager.StopWorkerAsync(request.WorkerType);

            return Ok(result);
        }
    }
}