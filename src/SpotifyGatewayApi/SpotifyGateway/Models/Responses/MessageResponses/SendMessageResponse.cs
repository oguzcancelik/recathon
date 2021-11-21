using Newtonsoft.Json;

namespace SpotifyGateway.Models.Responses.MessageResponses
{
    public class SendMessageResponse
    {
        [JsonProperty("message_id")] 
        public int MessageId { get; set; }
    }
}