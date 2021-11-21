using System.Collections.Generic;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Factories.Abstractions
{
    public interface ISettingFactory
    {
        List<ISettings> GetSettings(SettingsClass settingsClass);

        ISettings GetSetting(SettingsClass settingsClass);
    }
}