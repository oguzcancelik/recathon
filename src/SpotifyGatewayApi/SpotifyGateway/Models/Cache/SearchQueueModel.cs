using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Cache
{
    public class SearchQueueModel
    {
        public string PlaylistId { get; set; }

        public GenerateType GenerateType { get; set; }
    }
}