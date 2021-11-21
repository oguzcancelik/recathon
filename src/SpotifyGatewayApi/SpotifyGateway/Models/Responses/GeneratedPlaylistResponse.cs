using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpotifyGateway.Models.Responses
{
    public class GeneratedPlaylistResponse : PlaylistResponse
    {
        public string RedirectUrl { get; set; }

        public List<RecommendedTrackResponse> RecommendedTracks { get; set; }

        [JsonIgnore]
        public DateTime CreationTime { get; set; }
    }
}