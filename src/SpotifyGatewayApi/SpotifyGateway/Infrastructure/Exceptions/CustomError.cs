using Newtonsoft.Json;

namespace SpotifyGateway.Infrastructure.Exceptions
{
    public class CustomError
    {
        public int? Code { get; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public string Message { get; }

        public string UserFriendlyMessage { get; set; }

        public CustomError(string userFriendlyMessage)
        {
            Message = userFriendlyMessage;
            UserFriendlyMessage = userFriendlyMessage;
        }

        public CustomError(string message, string userFriendlyMessage)
        {
            Message = message;
            UserFriendlyMessage = userFriendlyMessage;
        }

        public CustomError(string message, int? code)
        {
            Message = message;
            Code = code;
        }

        public CustomError(string message, int? code, string userFriendlyMessage)
        {
            Message = message;
            Code = code;
            UserFriendlyMessage = userFriendlyMessage;
        }
    }
}