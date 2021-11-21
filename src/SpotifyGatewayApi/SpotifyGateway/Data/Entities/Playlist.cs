using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Data.Entities
{
    [Dapper.Contrib.Extensions.Table("playlist")]
    public class Playlist
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

        public bool IsPublic { get; set; }

        public bool IsCollaborative { get; set; }

        public int RecommendationCount { get; set; }

        public PlaylistType PlaylistType { get; set; }
        
        [NotMapped]
        public int TrackCount => Tracks?.Count ?? default;

        [NotMapped]
        public List<Track> Tracks { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsSearchReduced { get; set; }

        [NotMapped]
        public bool IsLockFailed { get; set; }

        public DateTime CreationTime { get; set; }
    }
}