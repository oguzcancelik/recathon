using System;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Events;

namespace SpotifyGateway.Managers
{
    public class WorkerManager : IWorkerManager
    {
        private readonly IRepository _repository;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IWorkerServiceFactory _workerServiceFactory;

        public WorkerManager(
            IRepository repository,
            IRedisCacheProvider redisCacheProvider,
            IWorkerServiceFactory workerServiceFactory
        )
        {
            _repository = repository;
            _redisCacheProvider = redisCacheProvider;
            _workerServiceFactory = workerServiceFactory;
        }

        public async Task<bool> StartWorkerAsync(WorkerType workerType, Worker worker = null)
        {
            var exists = WorkerConstants.Workers.TryGetValue(workerType.ToString(), out var workerId);

            if (!exists)
            {
                return false;
            }

            worker ??= await _repository.QueryFirstOrDefaultAsync<Worker>(QueryConstants.GetWorkerQuery, new { Id = workerId });

            if (worker is { IsEnabled: true, IsWorking: false })
            {
                var workerService = _workerServiceFactory.Get(workerType);

                switch (workerType)
                {
                    case WorkerType.Search:
                        WorkerHelpers.CreateRecurringJob(workerId, () => workerService.RunAsync(), WorkerConstants.SearchCronExpression, worker.TriggerImmediately);
                        await _redisCacheProvider.PubAsync(RedisConstants.UpdateSettingEvent, new UpdateSettingEventModel(SettingsClass.General, nameof(GeneralSettings.IsSearchWorkerEnabled), true));
                        break;
                    case WorkerType.Token:
                        await _redisCacheProvider.KeyDeleteAsync(RedisConstants.TokenRefreshTimeCache);
                        WorkerHelpers.CreateRecurringJob(workerId, () => workerService.RunAsync(), WorkerConstants.TokenCronExpression, worker.TriggerImmediately);
                        break;
                    case WorkerType.Category:
                        await _redisCacheProvider.InsertKeyAsync(RedisConstants.CategoryRefreshCache, RedisConstants.CategoryRefreshKeyCacheExpiryTime);
                        WorkerHelpers.CreateRecurringJob(workerId, () => workerService.RunAsync(), WorkerConstants.CategoryCronExpression, worker.TriggerImmediately);
                        break;
                    case WorkerType.NewDay:
                        WorkerHelpers.CreateRecurringJob(workerId, () => workerService.RunAsync(), WorkerConstants.NewDayCronExpression, worker.TriggerImmediately);
                        break;
                }

                var parameters = new { worker.Id, IsWorking = true, worker.IsEnabled, UpdateTime = DateTime.UtcNow };

                await _repository.ExecuteAsync(QueryConstants.UpdateWorkerQuery, parameters);

                return true;
            }

            return false;
        }

        public async Task<bool> StopWorkerAsync(WorkerType workerType)
        {
            var isExists = WorkerConstants.Workers.TryGetValue(workerType.ToString(), out var workerId);

            if (!isExists)
            {
                return false;
            }

            var worker = await _repository.QueryFirstOrDefaultAsync<Worker>(QueryConstants.GetWorkerQuery, new { Id = workerId });

            if (worker is { IsEnabled: false, IsWorking: true })
            {
                WorkerHelpers.RemoveRecurringJob(workerId);

                switch (workerType)
                {
                    case WorkerType.Search:
                        await _redisCacheProvider.PubAsync(RedisConstants.UpdateSettingEvent, new UpdateSettingEventModel(SettingsClass.General, nameof(GeneralSettings.IsSearchWorkerEnabled), false));
                        break;
                }

                var parameters = new { worker.Id, IsWorking = false, worker.IsEnabled, UpdateTime = DateTime.UtcNow };

                await _repository.ExecuteAsync(QueryConstants.UpdateWorkerQuery, parameters);

                return true;
            }

            return false;
        }

        public async Task RunWorkersOnStartupAsync()
        {
            var workers = await _repository.QueryAsync<Worker>(QueryConstants.GetWorkersToRunOnStartupQuery);

            foreach (var worker in workers)
            {
                if (worker.Name.TryParse(out WorkerType workerType))
                {
                    var isStarted = await StartWorkerAsync(workerType, worker);

                    /*
                     * worker will be triggered on StartWorker method if it's not already running.
                     * on the following block, if the worker is already running, it'll be triggered
                     */
                    if (!isStarted && worker.TriggerImmediately)
                    {
                        WorkerHelpers.TriggerRecurringJob(worker.Id);
                    }
                }
            }
        }
    }
}