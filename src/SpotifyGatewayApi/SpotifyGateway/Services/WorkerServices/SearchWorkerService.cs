using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Cache;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Events;
using SpotifyGateway.Models.MessageModels;
using SpotifyGateway.Models.Search;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyGateway.Services.WorkerServices.Abstractions;

namespace SpotifyGateway.Services.WorkerServices
{
    public class SearchWorkerService : ISearchWorkerService
    {
        private readonly IMessageServiceClient _messageServiceClient;
        private readonly ICredentialService _credentialService;
        private readonly IPlaylistService _playlistService;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ITelegramSettings _telegramSettings;
        private readonly ILoggerService _loggerService;

        public SearchWorkerService(
            IMessageServiceClient messageServiceClient,
            ICredentialService credentialService,
            IPlaylistService playlistService,
            IRedisCacheProvider redisCacheProvider,
            IRepository repository,
            ISpotifyAppContext spotifyAppContext,
            ITelegramSettings telegramSettings,
            ILoggerService loggerService
        )
        {
            _messageServiceClient = messageServiceClient;
            _redisCacheProvider = redisCacheProvider;
            _credentialService = credentialService;
            _playlistService = playlistService;
            _repository = repository;
            _spotifyAppContext = spotifyAppContext;
            _telegramSettings = telegramSettings;
            _loggerService = loggerService;
        }

        public async Task RunAsync()
        {
            var searchWorkerMessage = new SearchWorkerMessage
            {
                ClassName = nameof(SearchWorkerService),
                Subject = MessageSubjectType.SearchWorkerProcess
            };

            var model = await _redisCacheProvider.PopAsync<SearchQueueModel>(RedisConstants.SearchQueue);

            var shouldWorkerRun = model != default && !string.IsNullOrEmpty(model.PlaylistId);

            searchWorkerMessage.IsQueueEmpty = !shouldWorkerRun;

            if (!shouldWorkerRun)
            {
                model = await _repository.QueryFirstOrDefaultAsync<SearchQueueModel>(QueryConstants.GetPlaylistIdByRandomQuery);

                shouldWorkerRun = model != default && !string.IsNullOrEmpty(model.PlaylistId);
            }

            if (shouldWorkerRun)
            {
                try
                {
                    var credential = await _credentialService.GetByUsageTypeAsync(CredentialUsageType.Worker);

                    searchWorkerMessage.IsSpotifyApiValid = credential != default;

                    if (credential == default)
                    {
                        WorkerHelpers.RemoveRecurringJob(WorkerConstants.SearchWorker);

                        var parameters = new { Id = WorkerConstants.SearchWorker, IsWorking = false, IsEnabled = false, UpdateTime = DateTime.UtcNow };

                        await _repository.ExecuteAsync(QueryConstants.UpdateWorkerQuery, parameters);

                        await _redisCacheProvider.PubAsync(RedisConstants.UpdateSettingEvent, new UpdateSettingEventModel(SettingsClass.General, nameof(GeneralSettings.IsSearchWorkerEnabled), false));
                    }
                    else
                    {
                        _spotifyAppContext.Set(credential.ClientId, credential.AccessToken, credential.TokenType);

                        var searchModel = new SearchModel
                        {
                            PlaylistId = model.PlaylistId,
                            GenerateType = model.GenerateType,
                            SearchRange = SearchConstants.LargeSearchRange
                        };

                        var startTime = DateTime.UtcNow;

                        await _playlistService.SearchAsync(searchModel);

                        searchWorkerMessage.PlaylistId = model.PlaylistId;
                        searchWorkerMessage.TotalSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
                        searchWorkerMessage.IsSucceed = true;
                    }
                }
                catch (Exception e)
                {
                    searchWorkerMessage.IsSucceed = false;

                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(model.PlaylistId), model.PlaylistId }
                    };

                    await _loggerService.LogErrorAsync(e.Message, nameof(SearchWorkerService), nameof(RunAsync), logValues, e.StackTrace);
                }
            }

            if (_telegramSettings.IsSearchWorkerMessagesEnabled)
            {
                await _messageServiceClient.SendMessageAsync(searchWorkerMessage);
            }
        }
    }
}