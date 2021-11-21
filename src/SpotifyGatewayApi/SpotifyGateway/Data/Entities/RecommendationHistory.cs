using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Data.Entities
{
    [Table("recommendation_history")]
    public class RecommendationHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string PlaylistId { get; set; }

        public string PlaylistName { get; set; }

        public string GeneratedPlaylistId { get; set; }
        
        public string GeneratedPlaylistName { get; set; }

        public int RecommendedTrackCount { get; set; }

        public List<string> RecommendedTrackIds { get; set; }

        public bool IsSucceed => string.IsNullOrEmpty(ErrorMessage);

        public string ErrorMessage { get; set; }

        public PlaylistType PlaylistType { get; set; }

        public PlaylistSource PlaylistSource { get; set; }

        public GenerateType GenerateType { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}