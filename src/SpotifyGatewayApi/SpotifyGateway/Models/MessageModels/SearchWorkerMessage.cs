namespace SpotifyGateway.Models.MessageModels
{
    public class SearchWorkerMessage : BaseMessage
    {
        public bool IsQueueEmpty { get; set; }

        public bool? IsSpotifyApiValid { get; set; }

        public string PlaylistId { get; set; }

        public double? TotalSeconds { get; set; }

        public bool? IsSucceed { get; set; }
    }
}