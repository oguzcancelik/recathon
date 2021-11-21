using System.Collections.Generic;

namespace SpotifyGateway.Models.Responses
{
    public class CategoryResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImagePath { get; set; }

        public string IconPath { get; set; }

        public List<CategoryPlaylistResponse> Playlists { get; set; }
    }
}