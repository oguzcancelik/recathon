using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Models.Resources
{
    public class ErrorResources : BaseResource, IErrorResources
    {
        public string UserNotFound { get; set; }
    }
}