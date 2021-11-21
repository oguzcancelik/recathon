using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Managers.Abstractions
{
    public interface IWorkerManager
    {
        Task<bool> StartWorkerAsync(WorkerType workerType, Worker worker = null);

        Task<bool> StopWorkerAsync(WorkerType workerType);

        Task RunWorkersOnStartupAsync();
    }
}