using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Managers.Abstractions
{
    public interface IResourceManager
    {
        void UpdateResources(UpdateResourcesRequest request);
    }
}