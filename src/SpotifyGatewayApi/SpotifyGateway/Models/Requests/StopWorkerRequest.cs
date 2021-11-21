using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Requests
{
    public class StopWorkerRequest
    {
        public WorkerType WorkerType { get; set; }
    }
}