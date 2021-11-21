using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Search;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SearchType = SpotifyGateway.Models.Enums.SearchType;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class ArtistService : IArtistService
    {
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ILoggerService _loggerService;

        public ArtistService(
            IRedisCacheProvider redisCacheProvider,
            IRepository repository,
            ISpotifyAppContext spotifyAppContext,
            ILoggerService loggerService
        )
        {
            _redisCacheProvider = redisCacheProvider;
            _repository = repository;
            _spotifyAppContext = spotifyAppContext;
            _loggerService = loggerService;
        }

        public async Task<AlbumSearchResult> AlbumSearchAsync(SearchModel searchModel)
        {
            var searchResult = new AlbumSearchResult();

            var artistsForAlbumSearch = await _repository.QueryAsync<Artist>(
                QueryConstants.GetArtistsForAlbumSearchQuery,
                ParameterGenerator.SearchParameters(searchModel.PlaylistId, SearchType.Album, searchModel.SearchRange.AlbumSearchRange)
            );

            searchResult.SelectedCount = artistsForAlbumSearch.Count;

            artistsForAlbumSearch = artistsForAlbumSearch
                .Where(x =>
                {
                    var isLocked = _redisCacheProvider.Lock($"{RedisConstants.ArtistLock}{x.Id}", RedisConstants.SearchLockExpiryTime);

                    if (!isLocked)
                    {
                        return false;
                    }

                    x.CreationTime ??= DateTime.UtcNow;
                    x.UpdateTime = DateTime.UtcNow;
                    x.SearchAlbumType = ArtistHelpers.GetSearchAlbumType(x);

                    return x.SearchAlbumType != AlbumType.All;
                })
                .ToList();

            searchResult.SearchedCount = artistsForAlbumSearch.Count;

            if (artistsForAlbumSearch.Count == 0)
            {
                return searchResult;
            }

            var albums = await GetArtistAlbumsAsync(artistsForAlbumSearch);

            await _repository.InsertManyAsync(QueryConstants.InsertAlbumQuery, albums);

            await _repository.ExecuteAsync(QueryConstants.UpdateArtistQuery, artistsForAlbumSearch);

            searchResult.SucceededCount = artistsForAlbumSearch.Count;

            return searchResult;
        }

        private async Task<List<Album>> GetArtistAlbumsAsync(ICollection<Artist> artists)
        {
            var tasks = artists
                .Select(x =>
                {
                    var offset = x.SearchAlbumType switch
                    {
                        AlbumType.Album => x.AlbumOffset != -1 ? x.AlbumOffset : 0,
                        AlbumType.Single => x.SingleOffset != -1 ? x.SingleOffset : 0,
                        AlbumType.Compilation => x.CompilationOffset != -1 ? x.CompilationOffset : 0,
                        _ => x.AlbumOffset
                    };

                    return new
                    {
                        Artist = x,
                        Task = _spotifyAppContext.Api.GetArtistsAlbumsAsync(x.Id, x.SearchAlbumType, SpotifyApiConstants.ArtistAlbumsLimit, offset)
                    };
                })
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            var albums = new List<Album>();

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var artist = task.Artist;

                if (taskResult.HasError() || taskResult.Items == null)
                {
                    artists.Remove(artist);

                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        {nameof(SpotifyEndpoint), SpotifyEndpoint.GetArtistsAlbumsAsync},
                        {nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId},
                        {nameof(_spotifyAppContext.CredentialType), _spotifyAppContext.CredentialType},
                        {nameof(artist), artist}
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(ArtistService), nameof(GetArtistAlbumsAsync), logValues);

                    continue;
                }

                var taskResultTotal = taskResult.Total;
                var taskResultOffset = taskResult.Offset;
                var taskResultItemCount = taskResult.Items.Count;

                taskResult.Items = taskResult.Items
                    .Where(y => y.Artists is {Count: > 0} && y.Artists.First().Id == artist.Id)
                    .ToList();

                var result = taskResult.Items.ToEntity();

                switch (artist.SearchAlbumType)
                {
                    case AlbumType.Album:
                        artist.AlbumCount = taskResultTotal == 0 ? -1 : taskResultTotal;
                        artist.AlbumOffset = taskResultTotal == 0 ? -1 : taskResultOffset + taskResultItemCount;
                        artist.SavedAlbumCount = taskResultTotal == 0 ? -1 : artist.SavedAlbumCount + result.Count;
                        break;
                    case AlbumType.Single:
                        artist.SingleCount = taskResultTotal == 0 ? -1 : taskResultTotal;
                        artist.SingleOffset = taskResultTotal == 0 ? -1 : taskResultOffset + taskResultItemCount;
                        artist.SavedSingleCount = taskResultTotal == 0 ? -1 : artist.SavedSingleCount + result.Count;
                        break;
                    case AlbumType.Compilation:
                        artist.CompilationCount = taskResultTotal == 0 ? -1 : taskResultTotal;
                        artist.CompilationOffset = taskResultTotal == 0 ? -1 : taskResultOffset + taskResultItemCount;
                        artist.SavedCompilationCount = taskResultTotal == 0 ? -1 : artist.SavedCompilationCount + result.Count;
                        break;
                }

                albums.AddRange(result);
            }

            return albums;
        }
    }
}