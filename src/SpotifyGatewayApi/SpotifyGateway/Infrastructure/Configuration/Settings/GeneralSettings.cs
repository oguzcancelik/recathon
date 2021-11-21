using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Configuration.Settings
{
    public class GeneralSettings : IGeneralSettings
    {
        public bool IsSearchWorkerEnabled { get; set; }

        public int AdFrequency { get; set; }

        public string SpotifyPlaylistUrl { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}