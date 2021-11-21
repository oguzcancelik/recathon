using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;

namespace SpotifyGateway.Infrastructure.Configuration.Settings
{
    public class RecommendationSettings : IRecommendationSettings
    {
        public bool KeepUserLocked { get; set; }

        public bool IsAlbumSearchEnabled { get; set; }
    }
}