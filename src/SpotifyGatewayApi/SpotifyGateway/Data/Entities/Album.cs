using System;
using Dapper.Contrib.Extensions;

namespace SpotifyGateway.Data.Entities
{
    [Table("album")]
    public class Album
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int NumberOfTracks { get; set; }

        public bool IsCompleted { get; set; }

        public string Type { get; set; }

        public string ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}