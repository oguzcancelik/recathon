using System.Threading.Tasks;

namespace SpotifyGateway.Services.WorkerServices.Abstractions
{
    public interface IWorkerService
    {
        Task RunAsync();
    }
}