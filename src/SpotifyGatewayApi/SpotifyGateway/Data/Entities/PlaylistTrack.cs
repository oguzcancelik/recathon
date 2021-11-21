using Dapper.Contrib.Extensions;

namespace SpotifyGateway.Data.Entities
{
    [Table("playlist_track")]
    public class PlaylistTrack
    {
        public string Id { get; set; }

        public string PlaylistId { get; set; }

        public string TrackId { get; set; }

        public string Name { get; set; }

        public string ArtistId { get; set; }

        public string ArtistName { get; set; }

        public float Acousticness { get; set; }

        public float Danceability { get; set; }

        public int Duration { get; set; }

        public float Energy { get; set; }

        public float Instrumentalness { get; set; }

        public int Key { get; set; }

        public float Liveness { get; set; }

        public float Loudness { get; set; }

        public int Mode { get; set; }

        public float Speechiness { get; set; }

        public float Tempo { get; set; }

        public int TimeSignature { get; set; }

        public float Valence { get; set; }
    }
}