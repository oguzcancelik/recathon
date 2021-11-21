using Dapper.Contrib.Extensions;

namespace SpotifyGateway.Data.Entities
{
    [Table("related_artists")]
    public class RelatedArtist
    {
        public string Id { get; set; }

        public string ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string RelatedArtistId { get; set; }

        public string RelatedArtistName { get; set; }
    }
}