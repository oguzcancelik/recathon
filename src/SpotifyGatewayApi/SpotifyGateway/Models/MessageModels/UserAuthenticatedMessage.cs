namespace SpotifyGateway.Models.MessageModels
{
    public class UserAuthenticatedMessage : BaseMessage
    {
        public string UserId { get; set; }

        public string DisplayName { get; set; }
    }
}