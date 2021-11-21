namespace SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction
{
    public interface IBrowseSettings : ISettings
    {
        int CategoryPlaylistsExpiryHour { get; }
    }
}