using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Models.Resources
{
    public class BaseResource : IResources
    {
        public Language Language { get; set; }
    }
}