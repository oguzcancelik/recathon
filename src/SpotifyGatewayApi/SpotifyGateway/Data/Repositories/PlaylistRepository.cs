using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Data.Repositories
{
    public class PlaylistRepository : Repository, IPlaylistRepository
    {
        public PlaylistRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<List<RecommendedTrackResponse>> GetRecommendedTrackInformationAsync(List<string> trackIds)
        {
            return await QueryAsync<RecommendedTrackResponse>(QueryConstants.GetRecommendedTrackInformationQuery, new { TrackIds = trackIds });
        }
    }
}