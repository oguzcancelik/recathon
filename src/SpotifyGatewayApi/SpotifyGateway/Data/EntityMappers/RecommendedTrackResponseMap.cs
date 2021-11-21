using Dapper.FluentMap.Mapping;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Data.EntityMappers
{
    public class RecommendedTrackResponseMap : EntityMap<RecommendedTrackResponse>
    {
        public RecommendedTrackResponseMap()
        {
            Map(x => x.TrackName).ToColumn("name", false);
        }
    }
}