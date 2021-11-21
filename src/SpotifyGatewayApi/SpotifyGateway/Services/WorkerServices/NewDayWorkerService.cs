using System.Threading.Tasks;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Services.WorkerServices
{
    public class NewDayWorkerService : INewDayWorkerService
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly ISearchResultRepository _searchResultRepository;

        public NewDayWorkerService(
            ILoggerRepository loggerRepository,
            ISearchResultRepository searchResultRepository
        )
        {
            _loggerRepository = loggerRepository;
            _searchResultRepository = searchResultRepository;
        }

        public async Task RunAsync()
        {
            await _loggerRepository.DropCollectionAsync();
            await _searchResultRepository.DropCollectionAsync();
        }
    }
}