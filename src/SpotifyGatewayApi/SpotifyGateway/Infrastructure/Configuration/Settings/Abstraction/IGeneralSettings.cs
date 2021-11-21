using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction
{
    public interface IGeneralSettings : ISettings
    {
        bool IsSearchWorkerEnabled { get; set; }

        int AdFrequency { get; }

        string SpotifyPlaylistUrl { get; }

        LogLevel LogLevel { get; }
    }
}