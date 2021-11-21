using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Data.Entities
{
    public class SearchResult : BaseMongoDbEntity
    {
        public SearchModel SearchModel { get; set; }
        
        public RelatedArtistSearchResult RelatedArtistSearchResult { get; set; }

        public AlbumSearchResult AlbumSearchResult { get; set; }

        public TrackSearchResult TrackSearchResult { get; set; }

        public int TryCount { get; set; }
    }
}