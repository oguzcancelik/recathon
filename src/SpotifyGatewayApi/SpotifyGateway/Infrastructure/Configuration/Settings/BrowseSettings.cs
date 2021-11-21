using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;

namespace SpotifyGateway.Infrastructure.Configuration.Settings
{
    public class BrowseSettings : IBrowseSettings
    {
        public int CategoryPlaylistsExpiryHour { get; set; }
    }
}