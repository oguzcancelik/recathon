using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Search;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ITrackService _trackService;
        private readonly ILoggerService _loggerService;

        public AlbumService(
            IAlbumRepository albumRepository,
            IRedisCacheProvider redisCacheProvider,
            ISpotifyAppContext spotifyAppContext,
            ITrackService trackService,
            ILoggerService loggerService
        )
        {
            _albumRepository = albumRepository;
            _redisCacheProvider = redisCacheProvider;
            _spotifyAppContext = spotifyAppContext;
            _trackService = trackService;
            _loggerService = loggerService;
        }

        public async Task<TrackSearchResult> TrackSearchAsync(SearchModel searchModel)
        {
            var searchResult = new TrackSearchResult();

            var albumsForTrackSearch = await _albumRepository.GetAlbumsForTrackSearchAsync(searchModel);

            searchResult.SelectedCount = albumsForTrackSearch.Count;

            albumsForTrackSearch = albumsForTrackSearch
                .Where(x => _redisCacheProvider.Lock($"{RedisConstants.AlbumLock}{x.Id}", RedisConstants.SearchLockExpiryTime))
                .ToList();

            if (albumsForTrackSearch.Count > 20 && albumsForTrackSearch.Count % 20 < 10)
            {
                albumsForTrackSearch = albumsForTrackSearch.Take(albumsForTrackSearch.Count - albumsForTrackSearch.Count % 20).ToList();
            }

            searchResult.SearchedCount = albumsForTrackSearch.Count;

            if (albumsForTrackSearch.Count == 0)
            {
                return searchResult;
            }

            var tracks = await GetAlbumTracksAsync(albumsForTrackSearch);

            tracks = await _trackService.GetTrackAudioFeaturesAsync(tracks);

            var insertedAlbumIds = tracks.Select(x => x.AlbumId).Distinct().ToList();

            await _albumRepository.InsertManyAsync(QueryConstants.InsertTrackQuery, tracks);
            await _albumRepository.ExecuteAsync(QueryConstants.UpdateAlbumsQuery, new { AlbumIds = insertedAlbumIds, UpdateTime = DateTime.UtcNow });

            searchResult.SucceededCount = insertedAlbumIds.Count;

            return searchResult;
        }

        private async Task<List<Track>> GetAlbumTracksAsync(IReadOnlyCollection<Album> albums)
        {
            const int limit = SpotifyApiConstants.AlbumTracksLimit;

            var groupedAlbums = new List<List<Album>>();

            for (var i = 0; i < albums.Count; i += limit)
            {
                groupedAlbums.Add(albums.Skip(i).Take(limit).ToList());
            }

            var tasks = groupedAlbums
                .Select(x =>
                {
                    var albumIds = x.Select(y => y.Id).ToList();

                    return new
                    {
                        AlbumIds = albumIds,
                        Task = _spotifyAppContext.Api.GetSeveralAlbumsAsync(albumIds)
                    };
                })
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            var result = new List<Track>();

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var albumIds = task.AlbumIds;

                if (taskResult.HasError() || taskResult.Albums == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.GetSeveralAlbumsAsync },
                        { nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId },
                        { nameof(_spotifyAppContext.CredentialType), _spotifyAppContext.CredentialType },
                        { nameof(albumIds), albumIds }
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(AlbumService), nameof(GetAlbumTracksAsync), logValues);

                    continue;
                }

                result.AddRange(taskResult.Albums.SelectMany(x => x.GetTracks()));
            }

            return result;
        }
    }
}