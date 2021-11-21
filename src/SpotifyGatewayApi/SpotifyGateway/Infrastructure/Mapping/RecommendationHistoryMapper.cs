using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Procedures;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class RecommendationHistoryMapper
    {
        public static List<RecommendedTrackResponse> ToResponse(this IEnumerable<RecommendedTrackInformation> trackInformation)
        {
            return trackInformation.Select(x => x.ToResponse()).ToList();
        }

        public static RecommendedTrackResponse ToResponse(this RecommendedTrackInformation trackInformation)
        {
            return new RecommendedTrackResponse
            {
                Id = trackInformation.TrackId,
                ArtistName = trackInformation.ArtistName,
                ImagePath = trackInformation.ImagePath,
                TrackName = trackInformation.TrackName
            };
        }
    }
}