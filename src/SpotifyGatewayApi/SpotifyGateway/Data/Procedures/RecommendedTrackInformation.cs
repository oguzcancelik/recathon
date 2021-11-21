using System;

namespace SpotifyGateway.Data.Procedures
{
    public class RecommendedTrackInformation
    {
        public string GeneratedPlaylistId { get; set; }

        public string GeneratedPlaylistName { get; set; }

        public string PlaylistId { get; set; }

        public string TrackId { get; set; }

        public string TrackName { get; set; }

        public string ArtistName { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreationTime { get; set; }
    }
}