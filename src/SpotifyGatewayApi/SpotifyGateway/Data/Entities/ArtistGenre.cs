using System.Collections.Generic;

namespace SpotifyGateway.Data.Entities
{
    public class ArtistGenre
    {
        public string ArtistId { get; set; }

        public string ArtistName { get; set; }

        public List<string> Genres { get; set; }
    }
}