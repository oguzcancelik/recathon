using System;
using Newtonsoft.Json;
using SpotifyGateway.Infrastructure.JsonConverters;

namespace SpotifyGateway.Data.Entities
{
    [JsonConverter(typeof(TrackConverter))]
    public class Track
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string AlbumId { get; set; }

        public string AlbumName { get; set; }

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

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}