namespace SpotifyGateway.Models.Responses
{
    public class PlaylistResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int TrackCount { get; set; }

        public bool IsPublic { get; set; }

        public bool IsAd { get; set; }

        public T MemberwiseClone<T>() where T : PlaylistResponse
        {
            return (T)MemberwiseClone();
        }
    }
}