using Newtonsoft.Json;

namespace SpotifyGateway.Models.Responses.MessageResponses
{
    public class BaseTelegramResponse<T>
    {
        public bool Ok { get; set; }

        [JsonProperty("error_code")] 
        public int ErrorCode { get; set; }

        public string Description { get; set; }

        public T Result { get; set; }
    }
}