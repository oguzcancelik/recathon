using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace SpotifyGateway.Services.SpotifyServices
{
    public class RelatedArtistsService : IRelatedArtistsService
    {
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ILoggerService _loggerService;

        public RelatedArtistsService(
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

        public async Task<RelatedArtistSearchResult> RelatedArtistsSearchAsync(SearchModel searchModel)
        {
            var searchResult = new RelatedArtistSearchResult();

            var artistsForRelatedArtistsSearch = await _repository.QueryAsync<Artist>(
                QueryConstants.GetArtistsForRelatedArtistsSearchQuery,
                ParameterGenerator.SearchParameters(searchModel.PlaylistId, SearchType.RelatedArtists, searchModel.SearchRange.RelatedArtistSearchRange)
            );

            searchResult.SelectedCount = artistsForRelatedArtistsSearch.Count;

            artistsForRelatedArtistsSearch = artistsForRelatedArtistsSearch
                .Where(x => _redisCacheProvider.Lock($"{RedisConstants.RelatedArtistsLock}{x.Id}", RedisConstants.SearchLockExpiryTime))
                .ToList();

            searchResult.SearchedCount = artistsForRelatedArtistsSearch.Count;

            if (artistsForRelatedArtistsSearch.Count == 0)
            {
                return searchResult;
            }

            var (relatedArtists, artists, artistGenres) = await GetRelatedArtistsAsync(artistsForRelatedArtistsSearch);

            var tasks = new List<Task>
            {
                _repository.InsertManyAsync(QueryConstants.InsertRelatedArtistQuery, relatedArtists),
                _repository.InsertManyAsync(QueryConstants.UpsertArtistQuery, artists),
                _repository.InsertManyAsync(QueryConstants.InsertArtistGenreQuery, artistGenres)
            };

            await Task.WhenAll(tasks);

            searchResult.SucceededCount = artistsForRelatedArtistsSearch.Count;

            return searchResult;
        }

        private async Task<(List<RelatedArtist> RA, List<Artist> A, List<ArtistGenre> AG)> GetRelatedArtistsAsync(ICollection<Artist> artists)
        {
            var tasks = artists
                .Select(x => new {Artist = x, Task = _spotifyAppContext.Api.GetRelatedArtistsAsync(x.Id)})
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            var relatedArtists = new List<RelatedArtist>();
            var newArtists = new List<Artist>();
            var genres = new List<ArtistGenre>();

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var artist = task.Artist;

                if (taskResult.HasError() || taskResult.Artists == null)
                {
                    artists.Remove(artist);

                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        {nameof(SpotifyEndpoint), SpotifyEndpoint.GetRelatedArtistsAsync},
                        {nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId},
                        // {nameof(spotifyApiModel.CredentialType), spotifyApiModel.CredentialType},
                        {nameof(artist.Id), artist.Id},
                        {nameof(artist.Name), artist.Name}
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(RelatedArtistsService), nameof(GetRelatedArtistsAsync), logValues);

                    continue;
                }

                var result = new List<RelatedArtist> {artist.ToEntity()};

                var fullArtists = taskResult.Artists;

                if (fullArtists.Count > 0)
                {
                    result.AddRange(fullArtists.ToEntity(artist));
                    newArtists.AddRange(fullArtists.ToEntity());
                    genres.AddRange(fullArtists.ToArtistGenre());
                }

                relatedArtists.AddRange(result);
            }

            var remainingArtists = artists
                .Where(x => newArtists.All(y => y.Id != x.Id))
                .Select(x =>
                {
                    x.CreationTime = DateTime.UtcNow;
                    x.UpdateTime = DateTime.UtcNow;

                    return x;
                });

            newArtists.AddRange(remainingArtists);

            return (
                relatedArtists,
                newArtists.GroupBy(x => x.Id).Select(x => x.First()).ToList(),
                genres.GroupBy(x => x.ArtistId).Select(x => x.First()).ToList()
            );
        }
    }
}