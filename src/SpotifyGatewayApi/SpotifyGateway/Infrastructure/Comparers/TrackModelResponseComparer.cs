using System.Collections.Generic;
using SpotifyGateway.Models.Responses.ServiceResponses;

namespace SpotifyGateway.Infrastructure.Comparers
{
    public class TrackModelResponseComparer : IEqualityComparer<TrackModelResponse>
    {
        public bool Equals(TrackModelResponse x, TrackModelResponse y)
        {
            return x != null && y != null && x.TrackId == y.TrackId;
        }

        public int GetHashCode(TrackModelResponse obj)
        {
            return obj.TrackId.GetHashCode();
        }
    }
}