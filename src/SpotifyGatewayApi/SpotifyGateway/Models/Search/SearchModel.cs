using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Search
{
    public class SearchModel
    {
        public string PlaylistId { get; set; }

        public SearchRangeModel SearchRange { get; set; }

        public GenerateType GenerateType { get; set; }
    }
}