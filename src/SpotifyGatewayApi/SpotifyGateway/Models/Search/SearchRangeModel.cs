using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Search
{
    public class SearchRangeModel
    {
        public SearchRange RelatedArtistSearchRange { get; set; }

        public SearchRange AlbumSearchRange { get; set; }

        public SearchRange TrackSearchRange { get; set; }
    }
}