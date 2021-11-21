using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Factories
{
    public class SettingFactory : ISettingFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SettingFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<ISettings> GetSettings(SettingsClass settingsClass)
        {
            if (settingsClass == SettingsClass.All)
            {
                return GetAllSettings();
            }

            var settings = GetSetting(settingsClass);

            return new List<ISettings> { settings };
        }

        public ISettings GetSetting(SettingsClass settingsClass)
        {
            ISettings settings = settingsClass switch
            {
                SettingsClass.General => _serviceProvider.GetRequiredService<IGeneralSettings>(),
                SettingsClass.Auth => _serviceProvider.GetRequiredService<IAuthSettings>(),
                SettingsClass.Telegram => _serviceProvider.GetRequiredService<ITelegramSettings>(),
                SettingsClass.Browse => _serviceProvider.GetRequiredService<IBrowseSettings>(),
                SettingsClass.Recommendation => _serviceProvider.GetRequiredService<IRecommendationSettings>(),
                _ => null
            };

            return settings;
        }

        private List<ISettings> GetAllSettings()
        {
            var settingsList = _serviceProvider.GetServices<ISettings>();

            return settingsList.ToList();
        }
    }
}