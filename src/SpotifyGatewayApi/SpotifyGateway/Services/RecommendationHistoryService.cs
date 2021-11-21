using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Procedures;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.Abstractions;
using StackExchange.Redis;

namespace SpotifyGateway.Services
{
    public class RecommendationHistoryService : IRecommendationHistoryService
    {
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly IGeneralSettings _generalSettings;
        private readonly ISpotifyUserContext _spotifyUserContext;

        public RecommendationHistoryService(
            IRedisCacheProvider redisCacheProvider,
            IRepository repository,
            IGeneralSettings generalSettings,
            ISpotifyUserContext spotifyUserContext
        )
        {
            _redisCacheProvider = redisCacheProvider;
            _repository = repository;
            _generalSettings = generalSettings;
            _spotifyUserContext = spotifyUserContext;
        }

        public async Task<IEnumerable<string>> GetLastRecommendedTrackIdsAsync(string userId, string playlistId)
        {
            var trackIds = await _repository.QueryAsync<string>(
                QueryConstants.GetLastRecommendedTrackIdsQuery,
                new {UserId = userId, PlaylistId = playlistId}
            );

            return trackIds.Distinct();
        }

        public async Task<List<string>> GetAlternativeRecommendedTrackIdsAsync(string userId, string playlistId)
        {
            var trackIds = await _repository.QueryAsync<string>(
                QueryConstants.GetAlternativeTrackIdsQuery,
                new {UserId = userId, PlaylistId = playlistId}
            );

            return trackIds.Distinct().ToList();
        }

        public async Task InsertAsync(RecommendationHistory recommendationHistory)
        {
            var id = await _repository.QuerySingleAsync<int>(QueryConstants.InsertRecommendationHistoryQuery, recommendationHistory);

            if (recommendationHistory.RecommendedTrackIds != null && recommendationHistory.RecommendedTrackIds.Any())
            {
                var recommendedTracks = recommendationHistory.RecommendedTrackIds
                    .Select(x => new RecommendedTrack
                    {
                        RecommendationHistoryId = id,
                        TrackId = x,
                        CreationTime = DateTime.UtcNow
                    }).ToList();

                await _repository.ExecuteAsync(QueryConstants.InsertRecommendedTrackQuery, recommendedTracks);
            }
        }

        public async Task<BaseResponse<List<GeneratedPlaylistResponse>>> GetUsersRecommendationHistoryAsync()
        {
            var userId = _spotifyUserContext.UserId;
            
            var response = new BaseResponse<List<GeneratedPlaylistResponse>>();

            var playlists = await _redisCacheProvider.HashGetAllAsync<GeneratedPlaylistResponse>($"{RedisConstants.RecommendationHistory}{userId}");

            var isMarkedAsEmpty = await IsMarkedAsEmptyAsync(playlists, userId);

            if (isMarkedAsEmpty)
            {
                return response;
            }

            if (playlists == null || playlists.Count == 0)
            {
                var recommendedTrackInformationList = await _repository.QueryAsync<RecommendedTrackInformation>(
                    QueryConstants.GetUsersRecommendationHistoryQuery,
                    new {UserId = userId}
                );

                var data = recommendedTrackInformationList
                    .GroupBy(x => x.PlaylistId)
                    .ToDictionary(
                        x => x.Key,
                        x =>
                        {
                            var first = x.First();
                            var tracks = x.Select(y => y).ToList();

                            return new GeneratedPlaylistResponse
                            {
                                Id = first.GeneratedPlaylistId,
                                CreationTime = first.CreationTime,
                                IsAd = false,
                                Name = first.GeneratedPlaylistName,
                                RecommendedTracks = tracks.ToResponse(),
                                RedirectUrl = $"{_generalSettings.SpotifyPlaylistUrl}{first.GeneratedPlaylistId}",
                                TrackCount = tracks.Count
                            };
                        }
                    );

                if (data.Count > 0)
                {
                    var hashEntiries = data.Select(x => new HashEntry(x.Key, x.Value.ToJson())).ToArray();

                    await _redisCacheProvider.HashSetManyAsync($"{RedisConstants.RecommendationHistory}{userId}", hashEntiries, RedisConstants.HistoryExpiryTime);

                    playlists = data.Values.ToList();
                }
                else
                {
                    await _redisCacheProvider.HashSetAsync($"{RedisConstants.RecommendationHistory}{userId}", RedisConstants.EmptyCache, new {Id = RedisConstants.EmptyCache});

                    return response;
                }
            }
            else
            {
                playlists = playlists
                    .OrderByDescending(x => x.CreationTime)
                    .GroupBy(x => x.Name)
                    .Select(x => x.First())
                    .ToList();
            }

            response.Result = playlists;

            return response;
        }

        private async Task<bool> IsMarkedAsEmptyAsync(ICollection<GeneratedPlaylistResponse> playlists, string userId)
        {
            if (playlists == null || playlists.Count == 0)
            {
                return false;
            }

            var emptyCacheItem = playlists.FirstOrDefault(x => x.Id == RedisConstants.EmptyCache);

            if (emptyCacheItem == default)
            {
                return false;
            }

            if (playlists.Count == 1)
            {
                return true;
            }

            playlists.Remove(emptyCacheItem);

            await _redisCacheProvider.HashDeleteAsync($"{RedisConstants.RecommendationHistory}{userId}", RedisConstants.EmptyCache);

            return false;
        }
    }
}