using System;
using System.Threading.Tasks;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Services.WorkerServices
{
    public class CategoryWorkerService : ICategoryWorkerService
    {
        private readonly IBrowseService _browseService;
        private readonly ILoggerService _loggerService;

        public CategoryWorkerService(
            IBrowseService browseService,
            ILoggerService loggerService
        )
        {
            _browseService = browseService;
            _loggerService = loggerService;
        }

        public async Task RunAsync()
        {
            try
            {
                await _browseService.GetCategoryPlaylistsFromSpotifyApiAsync();
            }
            catch (Exception e)
            {
                await _loggerService.LogErrorAsync(e.Message, nameof(CategoryWorkerService), nameof(RunAsync), stackTrace: e.StackTrace);
            }
        }
    }
}