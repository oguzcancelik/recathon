using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Events;
using SpotifyGateway.Models.FilterModels;
using SpotifyGateway.Models.FilterModels.Base;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IRepository _repository;
        private readonly GeneralOptions _generalOptions;
        private readonly ISettingFactory _settingFactory;

        public ConfigurationManager(
            IRepository repository,
            IOptions<GeneralOptions> generalOptions,
            ISettingFactory settingFactory
        )
        {
            _repository = repository;
            _generalOptions = generalOptions.Value;
            _settingFactory = settingFactory;
        }

        public void UpdateSettings(UpdateSettingsRequest request)
        {
            var filterModel = request.SettingsClass != SettingsClass.All
                ? new ConfigurationFilterModel
                {
                    Where = x => new Where(
                        (nameof(x.Class), request.SettingsClass.ToString(), Operation.Contains),
                        (nameof(x.Application), _generalOptions.Name, Operation.Equal)
                    )
                }
                : new ConfigurationFilterModel
                {
                    Where = x => new Where((nameof(x.Application), _generalOptions.Name, Operation.Equal)),
                };

            var configurations = _repository.Query<Configuration>(filterModel.Query(), filterModel.Parameters);

            var settingsList = _settingFactory.GetSettings(request.SettingsClass);

            foreach (var settings in settingsList)
            {
                settings.SetValues(configurations);
            }
        }

        public void UpdateSetting(UpdateSettingEventModel model)
        {
            var settings = _settingFactory.GetSetting(model.SettingsClass);

            settings.SetValue(model.PropertyName, model.Value);
        }
    }
}