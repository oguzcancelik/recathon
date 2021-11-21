using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Data.Repositories
{
    public class AlbumRepository : Repository, IAlbumRepository
    {
        public AlbumRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<List<Album>> GetAlbumsForTrackSearchAsync(SearchModel searchModel)
        {
            var query = searchModel.GenerateType switch
            {
                GenerateType.All => QueryConstants.GetAlbumsForTrackSearchQuery,
                GenerateType.RecentlyReleased => QueryConstants.GetRecentlyReleasedAlbumsForTrackSearchQuery,
                _ => QueryConstants.GetAlbumsForTrackSearchQuery,
            };

            var result = await QueryAsync<Album>(query, ParameterGenerator.SearchParameters(searchModel.PlaylistId, SearchType.Track, searchModel.SearchRange.TrackSearchRange));

            if (result == null || result.Count < 5 || searchModel.SearchRange.TrackSearchRange == SearchRange.Small && result.Count < 20)
            {
                result = await QueryAsync<Album>(query, ParameterGenerator.SearchParameters(searchModel.PlaylistId, SearchType.Track, searchModel.SearchRange.TrackSearchRange, true));
            }

            return result;
        }
    }
}