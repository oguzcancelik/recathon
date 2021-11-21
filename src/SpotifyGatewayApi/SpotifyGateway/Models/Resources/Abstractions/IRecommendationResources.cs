namespace SpotifyGateway.Models.Resources.Abstractions
{
    public interface IRecommendationResources : IResources
    {
        string PlaylistDescription { get; set; }
    }
}