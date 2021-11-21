namespace SpotifyGateway.Models.Responses
{
    public class RecommendedTrackResponse
    {
        public string Id { get; set; }

        public string TrackName { get; set; }
        
        public string ArtistName { get; set; }

        public string ImagePath { get; set; }
    }
}