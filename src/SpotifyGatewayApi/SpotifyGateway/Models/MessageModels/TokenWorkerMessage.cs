namespace SpotifyGateway.Models.MessageModels
{
    public class TokenWorkerMessage : BaseMessage
    {
        public bool ShouldWorkerRun { get; set; }

        public bool? IsTokenRefreshTimeExceeded { get; set; }

        public bool? IsSucceed { get; set; }

        public bool? UpdatedCredentialsResult { get; set; }

        public int? UpdatedCredentialsCount { get; set; }
    }
}