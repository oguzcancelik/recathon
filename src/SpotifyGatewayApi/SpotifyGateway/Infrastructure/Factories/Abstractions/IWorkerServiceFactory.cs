using SpotifyGateway.Models.Enums;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Factories.Abstractions
{
    public interface IWorkerServiceFactory
    {
        IWorkerService Get(WorkerType workerType);
    }
}