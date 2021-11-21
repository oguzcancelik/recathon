using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction
{
    public interface ITelegramSettings : ISettings
    {
        string Token { get; }

        string ErrorLogChatId { get; }

        string InformationChatId { get; }

        TelegramLogLevel LogLevel { get; }

        bool IsTokenWorkerMessagesEnabled { get; }

        bool IsSearchWorkerMessagesEnabled { get; }

        bool IsNewUserAuthenticatedMessagesEnabled { get; }
    }
}