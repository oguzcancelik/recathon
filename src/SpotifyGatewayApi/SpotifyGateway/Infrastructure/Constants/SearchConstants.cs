using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class SearchConstants
    {
        public static readonly SearchRangeModel SmallSearchRange = new()
        {
            AlbumSearchRange = SearchRange.Small,
            TrackSearchRange = SearchRange.Small,
            RelatedArtistSearchRange = SearchRange.Small
        };

        public static readonly SearchRangeModel LargeSearchRange = new()
        {
            AlbumSearchRange = SearchRange.Large,
            TrackSearchRange = SearchRange.Large,
            RelatedArtistSearchRange = SearchRange.Large
        };
    }
}