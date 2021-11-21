using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Requests.ServiceRequests
{
    public class PredictionRequest
    {
        public string PlaylistId { get; set; }

        public string UserId { get; set; }

        public GenerateType GenerateType { get; set; }

        public int TryCount { get; set; }
    }
}