using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Models.Resources
{
    public class RecommendationResources : BaseResource, IRecommendationResources
    {
        public string PlaylistDescription { get; set; }
    }
}