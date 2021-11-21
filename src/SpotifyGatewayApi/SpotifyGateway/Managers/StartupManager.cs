using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.Events;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Managers
{
    public class StartupManager : IStartupManager
    {
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkerManager _workerManager;

        public StartupManager(
            IRedisCacheProvider redisCacheProvider,
            IServiceProvider serviceProvider,
            IWorkerManager workerManager
        )
        {
            _redisCacheProvider = redisCacheProvider;
            _serviceProvider = serviceProvider;
            _workerManager = workerManager;
        }

        public async Task RunAsync()
        {
            await _workerManager.RunWorkersOnStartupAsync();

            await _redisCacheProvider.SubAsync(RedisConstants.UpdateSettingsEvent, (_, message) =>
            {
                var request = message.FromJson<UpdateSettingsRequest>();

                using var scope = _serviceProvider.CreateScope();

                var configurationManager = scope.ServiceProvider.GetRequiredService<IConfigurationManager>();

                configurationManager.UpdateSettings(request);
            });

            await _redisCacheProvider.SubAsync(RedisConstants.UpdateSettingEvent, (_, message) =>
            {
                var model = message.FromJson<UpdateSettingEventModel>();

                using var scope = _serviceProvider.CreateScope();

                var configurationManager = scope.ServiceProvider.GetRequiredService<IConfigurationManager>();

                configurationManager.UpdateSetting(model);
            });

            await _redisCacheProvider.SubAsync(RedisConstants.UpdateResourcesEvent, (_, message) =>
            {
                var model = message.FromJson<UpdateResourcesRequest>();

                using var scope = _serviceProvider.CreateScope();

                var resourceManager = scope.ServiceProvider.GetRequiredService<IResourceManager>();

                resourceManager.UpdateResources(model);
            });
        }
    }
}