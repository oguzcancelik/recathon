namespace SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction
{
    public interface IRecommendationSettings : ISettings
    {
        bool KeepUserLocked { get; }

        bool IsAlbumSearchEnabled { get; }
    }
}