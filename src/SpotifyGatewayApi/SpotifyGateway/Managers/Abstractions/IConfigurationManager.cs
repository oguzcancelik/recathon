using SpotifyGateway.Models.Events;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Managers.Abstractions
{
    public interface IConfigurationManager
    {
        void UpdateSettings(UpdateSettingsRequest request);

        void UpdateSetting(UpdateSettingEventModel model);
    }
}