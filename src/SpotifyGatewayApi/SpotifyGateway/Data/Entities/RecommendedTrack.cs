using System;

namespace SpotifyGateway.Data.Entities
{
    public class RecommendedTrack
    {
        public int Id { get; set; }

        public int RecommendationHistoryId { get; set; }

        public string TrackId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}