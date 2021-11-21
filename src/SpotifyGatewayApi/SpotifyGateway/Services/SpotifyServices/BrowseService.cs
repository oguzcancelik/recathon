using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Cache;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class BrowseService : IBrowseService
    {
        private readonly IBrowseSettings _browseSettings;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly ICredentialService _credentialService;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ILoggerService _loggerService;

        public BrowseService(
            IBrowseSettings browseSettings,
            IRedisCacheProvider redisCacheProvider,
            ICredentialService credentialService,
            ISpotifyAppContext spotifyAppContext,
            ILoggerService loggerService
        )
        {
            _browseSettings = browseSettings;
            _redisCacheProvider = redisCacheProvider;
            _credentialService = credentialService;
            _spotifyAppContext = spotifyAppContext;
            _loggerService = loggerService;
        }

        public async Task<BaseResponse<List<CategoryResponse>>> GetCategoryPlaylistsAsync()
        {
            var categoryCacheModel = await _redisCacheProvider.GetAsync<CategoryCacheModel>(RedisConstants.CategoryPlaylistsCache);

            if (categoryCacheModel?.Responses == null || categoryCacheModel.Responses.Count == 0)
            {
                await GetCategoryPlaylistsFromSpotifyApiAsync();

                categoryCacheModel = await _redisCacheProvider.GetAsync<CategoryCacheModel>(RedisConstants.CategoryPlaylistsCache);
            }

            var response = new BaseResponse<List<CategoryResponse>>
            {
                Result = categoryCacheModel?.Responses
            };

            return response;
        }

        public async Task GetCategoryPlaylistsFromSpotifyApiAsync()
        {
            var categoryCacheModel = await _redisCacheProvider.GetAsync<CategoryCacheModel>(RedisConstants.CategoryPlaylistsCache);
            var isWorkerManuallyTriggered = await _redisCacheProvider.CheckExistsAndDeleteAsync(RedisConstants.CategoryRefreshCache);

            if (categoryCacheModel?.Responses != null
                && categoryCacheModel.Responses.Count > 0
                && (DateTime.UtcNow - categoryCacheModel.CreationTime).TotalHours < _browseSettings.CategoryPlaylistsExpiryHour
                && !isWorkerManuallyTriggered)
            {
                return;
            }

            var isLocked = await _redisCacheProvider.LockAsync(RedisConstants.CategoryPlaylistsLock, RedisConstants.CategoryPlaylistsLockExpiryTime);

            if (!isLocked)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                return;
            }

            var credential = await _credentialService.GetByUsageTypeAsync(CredentialUsageType.Worker);
            _spotifyAppContext.Set(credential.ClientId, credential.AccessToken, credential.TokenType);

            var categoryList = await _spotifyAppContext.Api.GetCategoriesAsync(SpotifyApiConstants.CategoryCountryCode, SpotifyApiConstants.CategoryLanguageCode, SpotifyApiConstants.CategoryLimit);

            if (categoryList.HasError() || categoryList.Categories?.Items == null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    {nameof(SpotifyEndpoint), SpotifyEndpoint.GetCategoriesAsync},
                    {nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId},
                    {nameof(_spotifyAppContext.CredentialType), _spotifyAppContext.CredentialType}
                };

                throw new CustomException(categoryList.Error, ErrorConstants.GetCategoriesError, nameof(BrowseService), nameof(GetCategoryPlaylistsFromSpotifyApiAsync), logValues);
            }

            var tasks = categoryList.Categories.Items
                .Select(x => new
                {
                    Category = x,
                    Task = _spotifyAppContext.Api.GetCategoryPlaylistsAsync(x.Id, limit: 50)
                })
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            var categories = new List<CategoryResponse>();

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var category = task.Category;

                if (taskResult.HasError() || taskResult.Playlists?.Items == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        {nameof(SpotifyEndpoint), SpotifyEndpoint.GetCategoryPlaylistsAsync},
                        {nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId},
                        {nameof(_spotifyAppContext.CredentialType), _spotifyAppContext.CredentialType},
                        {nameof(category.Id), category.Id},
                        {nameof(category.Name), category.Name}
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(BrowseService), nameof(GetCategoryPlaylistsFromSpotifyApiAsync), logValues);

                    continue;
                }

                var playlists = taskResult.Playlists.Items
                    .Where(y => y.Tracks.Total >= ApplicationConstants.TrackCountLimit)
                    .ToResponse()
                    .ToResponse(category.Id);

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImagePath = ImageHelpers.GetImagePath(category.Icons),
                    Playlists = playlists
                };

                categories.Add(categoryResponse);
            }

            categories = categories
                .Where(x => x.Playlists.Count > ApplicationConstants.CategoryPlaylistLimit)
                .ToList();

            await _redisCacheProvider.SetAsync(RedisConstants.CategoryPlaylistsCache, categories.ToCacheModel(), RedisConstants.CategoryPlaylistsCacheExpiryTime);
        }
    }
}