using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class MessageHelpers
    {
        public static bool AllowNull(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.ErrorLog => true,
                MessageType.Information => false,
                _ => true
            };
        }
    }
}