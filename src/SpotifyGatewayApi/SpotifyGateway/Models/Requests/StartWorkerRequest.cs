using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Requests
{
    public class StartWorkerRequest
    {
        public WorkerType WorkerType { get; set; }
    }
}