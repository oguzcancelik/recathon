using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Configuration.Settings
{
    public class TelegramSettings : ITelegramSettings
    {
        public string Token { get; set; }

        public string ErrorLogChatId { get; set; }

        public string InformationChatId { get; set; }

        public TelegramLogLevel LogLevel { get; set; }

        public bool IsTokenWorkerMessagesEnabled { get; set; }

        public bool IsSearchWorkerMessagesEnabled { get; set; }

        public bool IsNewUserAuthenticatedMessagesEnabled { get; set; }
    }
}