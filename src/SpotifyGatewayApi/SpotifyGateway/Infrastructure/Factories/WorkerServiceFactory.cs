using System;
using Microsoft.Extensions.DependencyInjection;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Factories
{
    public class WorkerServiceFactory : IWorkerServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkerServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IWorkerService Get(WorkerType workerType)
        {
            return workerType switch
            {
                WorkerType.Search => _serviceProvider.GetRequiredService<ISearchWorkerService>(),
                WorkerType.Token => _serviceProvider.GetRequiredService<ITokenWorkerService>(),
                WorkerType.Category => _serviceProvider.GetRequiredService<ICategoryWorkerService>(),
                WorkerType.NewDay => _serviceProvider.GetRequiredService<INewDayWorkerService>(),
                _ => null
            };
        }
    }
}