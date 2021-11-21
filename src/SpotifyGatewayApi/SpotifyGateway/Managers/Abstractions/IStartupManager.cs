using System.Threading.Tasks;

namespace SpotifyGateway.Managers.Abstractions
{
    public interface IStartupManager
    {
        Task RunAsync();
    }
}