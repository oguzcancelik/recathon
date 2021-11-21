using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Requests
{
    public class UpdateResourcesRequest
    {
        public ResourcesClass ResourcesClass { get; set; }

        public Language? Language { get; set; }
    }
}