using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Infrastructure.Comparers;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Requests.ServiceRequests;
using SpotifyGateway.Models.Resources.Abstractions;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Models.Responses.ServiceResponses;
using SpotifyGateway.Models.SavedData;
using SpotifyGateway.Models.Search;
using SpotifyGateway.ServiceClients.Abstractions;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using PlaylistTrack = SpotifyAPI.Web.Models.PlaylistTrack;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IAlbumService _albumService;
        private readonly IArtistService _artistService;
        private readonly IPredictionServiceClient _predictionServiceClient;
        private readonly IPlaylistRepository _repository;
        private readonly IRecommendationHistoryService _recommendationHistoryService;
        private readonly IRecommendationResources _recommendationResources;
        private readonly IRecommendationSettings _recommendationSettings;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRelatedArtistsService _relatedArtistsService;
        private readonly ISearchResultRepository _searchResultRepository;
        private readonly IGeneralSettings _generalSettings;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ISpotifyUserContext _spotifyUserContext;
        private readonly ITrackService _trackService;
        private readonly IUserService _userService;
        private readonly ILoggerService _loggerService;
        private PlaylistType _playlistType;
        private GenerateType _generateType;
        private int _tryCount;

        public PlaylistService(
            IAlbumService albumService,
            IArtistService artistService,
            IPredictionServiceClient predictionServiceClient,
            IPlaylistRepository repository,
            IRecommendationHistoryService recommendationHistoryService,
            IRecommendationResources recommendationResources,
            IRecommendationSettings recommendationSettings,
            IRedisCacheProvider redisCacheProvider,
            IRelatedArtistsService relatedArtistsService,
            ISearchResultRepository searchResultRepository,
            IGeneralSettings generalSettings,
            ISpotifyAppContext spotifyAppContext,
            ISpotifyUserContext spotifyUserContext,
            ITrackService trackService,
            IUserService userService,
            ILoggerService loggerService
        )
        {
            _albumService = albumService;
            _artistService = artistService;
            _redisCacheProvider = redisCacheProvider;
            _predictionServiceClient = predictionServiceClient;
            _repository = repository;
            _recommendationHistoryService = recommendationHistoryService;
            _recommendationResources = recommendationResources;
            _recommendationSettings = recommendationSettings;
            _relatedArtistsService = relatedArtistsService;
            _searchResultRepository = searchResultRepository;
            _generalSettings = generalSettings;
            _spotifyAppContext = spotifyAppContext;
            _spotifyUserContext = spotifyUserContext;
            _trackService = trackService;
            _userService = userService;
            _loggerService = loggerService;
        }

        public async Task<BaseResponse<GeneratedPlaylistResponse>> GeneratePlaylistAsync(GeneratePlaylistRequest request)
        {
            request.PlaylistType = PlaylistHelpers.GetPlaylistType(request.PlaylistId);

            if (request.PlaylistType != PlaylistType.Playlist)
            {
                request.PlaylistId = $"{_spotifyUserContext.UserId}-{request.PlaylistId}";
            }

            var response = new BaseResponse<GeneratedPlaylistResponse>();

            var isLocked = await _redisCacheProvider.LockAsync($"{RedisConstants.UserLock}{_spotifyUserContext.UserId}", RedisConstants.DefaultExpiryTime);

            if (!isLocked)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(request.PlaylistId), request.PlaylistId }
                };

                await _loggerService.LogWarningAsync(ErrorConstants.UserIsNotAvailable, nameof(PlaylistService), nameof(GeneratePlaylistAsync), logValues);

                response.Errors.Add(new CustomError(ErrorConstants.UserIsNotAvailable));

                return response;
            }

            var startTime = DateTime.UtcNow;

            try
            {
                _playlistType = request.PlaylistType;
                _generateType = request.GenerateType;

                var playlist = await GetPlaylistAsync(request.PlaylistId);

                var searchRange = await CalculateSearchRange(playlist);

                var searchModel = new SearchModel
                {
                    PlaylistId = playlist.Id,
                    GenerateType = _generateType,
                    SearchRange = searchRange
                };

                var recommendedTracks = await SearchAndRecommendAsync(searchModel);

                var recommendedTracksBackup = recommendedTracks;

                const int limit = DatabaseConstants.MinRecommendationLimit;

                for (var i = 0; i < 2 && recommendedTracks.Count < limit && recommendedTracksBackup.Count < limit; i++)
                {
                    recommendedTracks = await SearchAndRecommendAsync(searchModel);

                    recommendedTracksBackup.AddRange(recommendedTracks);
                    recommendedTracksBackup = recommendedTracksBackup.Distinct().ToList();
                }

                for (var i = 0; i < 2 && recommendedTracks.Count < limit && recommendedTracksBackup.Count < limit; i++)
                {
                    recommendedTracks = await GetRecommendedTracksAsync(playlist.Id);

                    recommendedTracksBackup.AddRange(recommendedTracks);
                    recommendedTracksBackup = recommendedTracksBackup.Distinct().ToList();
                }

                if (recommendedTracks.Count < limit)
                {
                    recommendedTracks = recommendedTracksBackup;
                }

                if (recommendedTracks.Count < limit)
                {
                    var alternativeTrackIds = await _recommendationHistoryService.GetAlternativeRecommendedTrackIdsAsync(_spotifyUserContext.UserId, request.PlaylistId);

                    if (alternativeTrackIds is { Count: > 0 })
                    {
                        recommendedTracks.AddRange(alternativeTrackIds);

                        recommendedTracks = recommendedTracks
                            .Shuffle()
                            .Distinct()
                            .Take(DatabaseConstants.MaxRecommendationLimit)
                            .ToList();
                    }
                }

                response.Result = await CreatePlaylistAsync(playlist.Name, recommendedTracks);
                response.Result.RecommendedTracks = await _repository.GetRecommendedTrackInformationAsync(recommendedTracks);
                response.Result.RedirectUrl = $"{_generalSettings.SpotifyPlaylistUrl}{response.Result.Id}";
                response.Result.CreationTime = DateTime.UtcNow;

                if (!_recommendationSettings.KeepUserLocked)
                {
                    await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserLock}{_spotifyUserContext.UserId}");
                }

                var recommendationHistory = new RecommendationHistory
                {
                    UserId = _spotifyUserContext.UserId,
                    PlaylistId = request.PlaylistId,
                    PlaylistName = playlist.Name,
                    GeneratedPlaylistId = response.Result.Id,
                    GeneratedPlaylistName = response.Result.Name,
                    RecommendedTrackCount = recommendedTracks.Count,
                    RecommendedTrackIds = recommendedTracks,
                    PlaylistType = request.PlaylistType,
                    PlaylistSource = request.PlaylistSource,
                    GenerateType = request.GenerateType,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    CreationTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow
                };

                await _recommendationHistoryService.InsertAsync(recommendationHistory);
                await _redisCacheProvider.HashSetAsync($"{RedisConstants.RecommendationHistory}{_spotifyUserContext.UserId}", request.PlaylistId, response.Result, RedisConstants.HistoryExpiryTime);

                if (_generalSettings.IsSearchWorkerEnabled)
                {
                    await _redisCacheProvider.PushAsync(RedisConstants.SearchQueue, request.ToQueueModel().ToJson());
                }
            }
            catch (Exception e)
            {
                await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserLock}{_spotifyUserContext.UserId}");

                if (request.ShowAd)
                {
                    await _redisCacheProvider.InsertKeyAsync($"{RedisConstants.AdFreeUser}{_spotifyUserContext.UserId}", RedisConstants.AdFreeUserKeyExpiryTime);
                }

                var recommendationHistory = new RecommendationHistory
                {
                    UserId = _spotifyUserContext.UserId,
                    PlaylistId = request.PlaylistId,
                    ErrorMessage = e.ToErrorMessage().Limit(DatabaseConstants.ErrorMessageLimit),
                    RecommendedTrackCount = 0,
                    PlaylistType = request.PlaylistType,
                    PlaylistSource = request.PlaylistSource,
                    GenerateType = request.GenerateType,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    CreationTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow
                };

                await _recommendationHistoryService.InsertAsync(recommendationHistory);

                throw;
            }

            return response;
        }

        public async Task SearchAsync(SearchModel searchModel)
        {
            if (_spotifyAppContext?.Api == null)
            {
                return;
            }

            var relatedArtistSearchResult = await _relatedArtistsService.RelatedArtistsSearchAsync(searchModel);

            var albumSearchResult = _recommendationSettings.IsAlbumSearchEnabled
                ? await _artistService.AlbumSearchAsync(searchModel)
                : null;

            var trackSearchResult = await _albumService.TrackSearchAsync(searchModel);

            var searchResult = new SearchResult
            {
                SearchModel = searchModel,
                RelatedArtistSearchResult = relatedArtistSearchResult,
                AlbumSearchResult = albumSearchResult,
                TrackSearchResult = trackSearchResult,
                TryCount = _tryCount
            };

            await _searchResultRepository.InsertAsync(searchResult);
        }

        private async Task<List<string>> SearchAndRecommendAsync(SearchModel searchModel)
        {
            await SearchAsync(searchModel);

            var recommendedTracks = await GetRecommendedTracksAsync(searchModel.PlaylistId);

            return recommendedTracks;
        }


        private async Task<GeneratedPlaylistResponse> CreatePlaylistAsync(string playlistName, IReadOnlyCollection<string> trackIds)
        {
            if (trackIds.Count < DatabaseConstants.MinRecommendationLimit)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(playlistName), playlistName },
                    { nameof(trackIds.Count), trackIds.Count },
                    { nameof(trackIds), trackIds }
                };

                throw new CustomException(ErrorConstants.NumberOfRecommendedTrackIsNotEnough, ErrorConstants.AddTracksPlaylistError, nameof(PlaylistService), nameof(CreatePlaylistAsync), logValues);
            }

            var generatedPlaylistName = $"{ApplicationConstants.AppPrefix}{playlistName}";

            var userPlaylists = await _userService.GetCurrentUsersPlaylistsAsync();
            var existingPlaylist = userPlaylists.Result?.FirstOrDefault(x => x.Name == generatedPlaylistName);

            GeneratedPlaylistResponse response;

            if (existingPlaylist != default)
            {
                var playlistTracks = await GetPlaylistTracksAsync(existingPlaylist.Id, existingPlaylist.IsPublic, fields: SpotifyApiConstants.PlaylistTrackIdsRequestedFields);

                const int limit = SpotifyApiConstants.RemovePlaylistTracksLimit;

                for (var i = 0; i < playlistTracks.Count; i += limit)
                {
                    var deleteTrackUris = playlistTracks.Skip(i).Take(limit).Select(x => x.Track.Id).ToDeleteTrackUri();
                    var removeTracksErrorResponse = await _spotifyUserContext.Api.RemovePlaylistTracksAsync(existingPlaylist.Id, deleteTrackUris);

                    if (removeTracksErrorResponse?.Error != null)
                    {
                        var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                            { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                            { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                            { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                            { nameof(SpotifyEndpoint), SpotifyEndpoint.RemovePlaylistTracksAsync },
                            { nameof(existingPlaylist.Id), existingPlaylist.Id },
                            { nameof(playlistName), playlistName },
                            { nameof(playlistTracks.Count), playlistTracks.Count },
                            { nameof(deleteTrackUris), deleteTrackUris }
                        };

                        await _loggerService.LogErrorAsync(removeTracksErrorResponse.Error, nameof(PlaylistService), nameof(CreatePlaylistAsync), logValues);
                    }
                }

                response = existingPlaylist.ToResponse();
            }
            else
            {
                var playlistDescription = _recommendationResources.PlaylistDescription.Format(_spotifyUserContext.User.DisplayName ?? _spotifyUserContext.UserId);

                var createdPlaylist = await _spotifyUserContext.Api.CreatePlaylistAsync(_spotifyUserContext.UserId, generatedPlaylistName, false, false, playlistDescription);

                if (createdPlaylist.HasError())
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                        { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                        { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                        { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.CreatePlaylistAsync },
                        { nameof(playlistName), playlistName }
                    };

                    throw new CustomException(createdPlaylist.Error, ErrorConstants.CreatePlaylistError, nameof(PlaylistService), nameof(CreatePlaylistAsync), logValues);
                }

                response = createdPlaylist.ToResponse();
            }

            var addTracksErrorResponse = await _spotifyUserContext.Api.AddPlaylistTracksAsync(response.Id, trackIds.ToUri());

            if (addTracksErrorResponse?.Error != null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(SpotifyEndpoint), SpotifyEndpoint.AddPlaylistTracksAsync },
                    { nameof(response.Id), response.Id },
                    { nameof(trackIds), trackIds }
                };

                throw new CustomException(addTracksErrorResponse.Error, ErrorConstants.AddTracksPlaylistError, nameof(PlaylistService), nameof(CreatePlaylistAsync), logValues);
            }

            response.TrackCount = trackIds.Count;

            return response;
        }

        private async Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            var playlist = await _redisCacheProvider.GetAsync<Playlist>($"{RedisConstants.PlaylistCache}{playlistId}")
                           ?? await _repository.QueryFirstOrDefaultAsync<Playlist>(QueryConstants.GetPlaylistQuery, new { Id = playlistId });

            if (playlist == null || PlaylistHelpers.IsExpired(_playlistType, playlist.LastUpdated))
            {
                await _repository.ExecuteAsync(QueryConstants.DeletePlaylistTracksQuery, new { PlaylistId = playlistId });

                var newPlaylist = await GetPlaylistFromSpotifyApiAsync(playlistId);

                playlist = playlist != null ? playlist.Merge(newPlaylist) : newPlaylist;

                await _redisCacheProvider.SetAsync($"{RedisConstants.PlaylistCache}{playlistId}", playlist, CacheHelpers.GetPlaylistExpiryTime(playlist.PlaylistType));
            }
            else if (playlist.Tracks == null || playlist.Tracks.Count == 0)
            {
                var playlistTracks = await _repository.QueryAsync<Data.Entities.PlaylistTrack>(QueryConstants.GetPlaylistTracksQuery, new { PlaylistId = playlistId });

                if (playlistTracks.Count > 0)
                {
                    playlist.Tracks = playlistTracks.ToEntity();
                }
                else
                {
                    var newPlaylist = await GetPlaylistFromSpotifyApiAsync(playlistId);

                    playlist = playlist.Merge(newPlaylist);
                }

                await _redisCacheProvider.SetAsync($"{RedisConstants.PlaylistCache}{playlistId}", playlist, CacheHelpers.GetPlaylistExpiryTime(playlist.PlaylistType));
            }

            await _repository.ExecuteAsync(QueryConstants.UpdatePlaylistIncreaseRecommendationCountQuery, new { playlist.Id });

            return playlist;
        }

        private async Task<Playlist> GetPlaylistFromSpotifyApiAsync(string playlistId)
        {
            var isInsufficientPlaylist = await _redisCacheProvider.KeyExistsAsync($"{RedisConstants.InsufficientPlaylistCache}{playlistId}");

            if (isInsufficientPlaylist)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(playlistId), playlistId }
                };

                var errorMessage = ErrorConstants.TrackCountNotEnough.Format(_playlistType.ToString().ToLower());

                throw new CustomException(errorMessage, errorMessage, nameof(PlaylistService), nameof(GetPlaylistFromSpotifyApiAsync), logValues);
            }

            var playlist = await GetPlaylistByTypeAsync(playlistId);

            if (playlist.TrackCount >= ApplicationConstants.TrackCountLimit)
            {
                playlist.Tracks = await _trackService.GetTrackAudioFeaturesAsync(playlist.Tracks, true);
            }

            if (playlist.TrackCount < ApplicationConstants.TrackCountLimit)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(playlistId), playlistId },
                    { nameof(playlist.Name), playlist.Name },
                    { nameof(_playlistType), _playlistType },
                    { nameof(playlist.IsLockFailed), playlist.IsLockFailed },
                    { nameof(playlist.TrackCount), playlist.TrackCount },
                    { nameof(ApplicationConstants.TrackCountLimit), ApplicationConstants.TrackCountLimit }
                };

                await _redisCacheProvider.InsertKeyAsync($"{RedisConstants.InsufficientPlaylistCache}{playlistId}", RedisConstants.InsufficientPlaylistKeyExpiryTime);

                var errorMessage = ErrorConstants.TrackCountNotEnough.Format(_playlistType.ToString().ToLower());

                throw new CustomException(errorMessage, errorMessage, nameof(PlaylistService), nameof(GetPlaylistFromSpotifyApiAsync), logValues);
            }

            var playlistTracks = playlist.Tracks.ToEntity(playlist.Id);

            await _repository.InsertManyAsync(QueryConstants.InsertPlaylistTrackQuery, playlistTracks);

            await _repository.ExecuteAsync(QueryConstants.UpsertPlaylistQuery, playlist);

            return playlist;
        }

        private async Task<Playlist> GetPlaylistByTypeAsync(string playlistId)
        {
            return _playlistType switch
            {
                PlaylistType.Playlist => await GetCurrentUsersPlaylistAsync(playlistId),
                PlaylistType.Saved => await _userService.GetCurrentUsersSavedTracksAsync(playlistId),
                PlaylistType.Top => await _userService.GetCurrentUsersTopTracksAsync(playlistId),
                PlaylistType.Recent => await _userService.GetCurrentUsersRecentTracksAsync(playlistId),
                _ => default
            };
        }

        private async Task<Playlist> GetCurrentUsersPlaylistAsync(string playlistId)
        {
            var fullPlaylist = await _spotifyUserContext.Api.GetPlaylistAsync(playlistId, fields: SpotifyApiConstants.PlaylistRequestedFields);

            if (fullPlaylist.HasError())
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(SpotifyEndpoint), SpotifyEndpoint.GetPlaylistAsync },
                    { nameof(playlistId), playlistId }
                };

                throw new CustomException(fullPlaylist.Error, ErrorConstants.UserPlaylistError, nameof(PlaylistService), nameof(GetCurrentUsersPlaylistAsync), logValues);
            }

            var playlist = fullPlaylist.ToEntity();

            var result = fullPlaylist.Tracks;

            var indexes = PlaylistHelpers.CalculateIndexes(result.Total, PlaylistType.Playlist);

            if (indexes.Count > 0)
            {
                var playlistTracks = await GetPlaylistTracksAsync(playlistId, fullPlaylist.Public, indexes);

                result.Items.AddRange(playlistTracks);
            }

            if (result.HasError() || result.Items == null || result.Items.Count == 0)
            {
                return playlist;
            }

            playlist.Tracks = result.Items
                .Where(x => !x.IsLocal)
                .Select(x => x.Track)
                .ToEntity();

            return playlist;
        }

        private async Task<List<PlaylistTrack>> GetPlaylistTracksAsync(
            string playlistId,
            bool isPublic,
            ICollection<int> indexes = null,
            string fields = SpotifyApiConstants.PlaylistTracksRequestedFields
        )
        {
            var playlistTracks = new List<PlaylistTrack>();

            if (indexes == null)
            {
                indexes = new List<int> { 0 };
            }
            else if (indexes.Count == 0)
            {
                indexes.Add(0);
            }

            var spotifyApi = isPublic ? _spotifyAppContext.Api : _spotifyUserContext.Api;

            var tasks = indexes
                .Select(x => spotifyApi.GetPlaylistTracksAsync(playlistId, fields, SpotifyApiConstants.PlaylistTracksLimit, x))
                .ToList();

            await Task.WhenAll(tasks);

            foreach (var taskResult in tasks.Select(task => task.Result))
            {
                if (taskResult.HasError() || taskResult.Items == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                        { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                        { nameof(_spotifyUserContext.User.ClientId), isPublic ? _spotifyAppContext.ClientId : _spotifyUserContext.User.ClientId },
                        { nameof(_spotifyUserContext.CredentialType), isPublic ? _spotifyAppContext.CredentialType : _spotifyUserContext.CredentialType },
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.GetPlaylistTracksAsync },
                        { nameof(playlistId), playlistId },
                        { nameof(isPublic), isPublic }
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(PlaylistService), nameof(GetPlaylistTracksAsync), logValues);
                }
                else
                {
                    playlistTracks.AddRange(taskResult.Items);
                }
            }

            return playlistTracks;
        }

        private async Task<List<string>> GetRecommendedTracksAsync(string playlistId)
        {
            var request = new PredictionRequest
            {
                PlaylistId = playlistId,
                UserId = _spotifyUserContext.UserId,
                GenerateType = _generateType,
                TryCount = ++_tryCount
            };

            var modelResults = await _predictionServiceClient.GetModelResultsAsync(request);

            var areMultipleMethodsEnabled = modelResults.Count > 1;

            var recommendedTracks = modelResults.FirstOrDefault().Value;

            if (areMultipleMethodsEnabled)
            {
                recommendedTracks = modelResults
                    .Aggregate(
                        recommendedTracks,
                        (current, pair) => current.Intersect(pair.Value, new TrackModelResponseComparer()).ToList()
                    );
            }

            var maxLimitValidation = recommendedTracks.Count < DatabaseConstants.MaxRecommendationLimit;
            var minLimitValidation = recommendedTracks.Count > DatabaseConstants.MinRecommendationLimit || !areMultipleMethodsEnabled;

            if (maxLimitValidation && minLimitValidation)
            {
                return recommendedTracks.Select(x => x.TrackId).ToList();
            }

            if (!minLimitValidation)
            {
                recommendedTracks = modelResults.FirstOrDefault().Value;

                maxLimitValidation = recommendedTracks.Count < DatabaseConstants.MaxRecommendationLimit;
            }

            if (maxLimitValidation)
            {
                return recommendedTracks.Select(x => x.TrackId).ToList();
            }

            recommendedTracks = GetSelectedTracks(recommendedTracks);

            return recommendedTracks
                .Shuffle()
                .Take(DatabaseConstants.MaxRecommendationLimit)
                .Select(x => x.TrackId)
                .ToList();
        }

        private static List<TrackModelResponse> GetSelectedTracks(IEnumerable<TrackModelResponse> recommendedTracks)
        {
            var random = new Random();

            return recommendedTracks
                .GroupBy(x => x.ArtistId)
                .SelectMany(x =>
                {
                    var count = x.Count();
                    var selectedTracks = new List<TrackModelResponse>();

                    var allowMultipleTracks = ListHelpers.GenerateRandomBoolean(random);
                    var maxAllowedTrackCount = allowMultipleTracks ? 2 : 1;

                    switch (count)
                    {
                        case 1:
                        case 2 when allowMultipleTracks:
                            selectedTracks.AddRange(x);
                            break;
                        default:
                        {
                            var indexes = ListHelpers.GenerateRandomIndexes(maxAllowedTrackCount, count, random: random);

                            selectedTracks.AddRange(indexes.Select(x.ElementAt));
                            break;
                        }
                    }

                    return selectedTracks;
                })
                .ToList();
        }

        private async Task<SearchRangeModel> CalculateSearchRange(Playlist playlist)
        {
            var searchRangeModel = SearchConstants.SmallSearchRange;

            if (playlist.IsSearchReduced && playlist.PlaylistType != PlaylistType.Recent)
            {
                return searchRangeModel;
            }

            var playlistArtistCount = playlist.Tracks.Select(x => x.ArtistId).Distinct().Count();

            var query = $"{QueryConstants.GetSavedRelatedArtistInformationQuery}{QueryConstants.GetSavedAlbumInformationQuery}{QueryConstants.GetSavedTrackInformationQuery}";

            var result = await _repository.QueryMultipleAsync<SavedRelatedArtistData, SavedAlbumData, SavedTrackData>(query, new { PlaylistId = playlist.Id });

            searchRangeModel = new SearchRangeModel
            {
                RelatedArtistSearchRange = PlaylistHelpers.CalculateSearchRangeForRelatedArtist(result.First, playlistArtistCount),
                AlbumSearchRange = PlaylistHelpers.CalculateSearchRangeForAlbum(result.Second, playlistArtistCount),
                TrackSearchRange = PlaylistHelpers.CalculateSearchRangeForTrack(result.Third, playlistArtistCount)
            };

            var reduceSearch = !playlist.IsSearchReduced
                               && playlist.PlaylistType != PlaylistType.Recent
                               && searchRangeModel.RelatedArtistSearchRange == SearchRange.Small
                               && searchRangeModel.AlbumSearchRange == SearchRange.Small
                               && searchRangeModel.TrackSearchRange == SearchRange.Small;

            if (reduceSearch)
            {
                await _repository.ExecuteAsync(QueryConstants.UpdatePlaylistReduceSearchQuery, new { playlist.Id });
            }

            return searchRangeModel;
        }
    }
}