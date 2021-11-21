using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.MessageModels
{
    public class BaseMessage
    {
        public string ClassName { get; set; }

        public MessageSubjectType? Subject { get; set; }
    }
}