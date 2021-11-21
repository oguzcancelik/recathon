using SpotifyGateway.Models.Api;
using SpotifyGateway.Models.Api.Abstractions;

namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class TelegramServiceOptions : IApiOptions
    {
        public string Url { get; set; }

        public ApiAction SendMessageAction { get; set; }
    }
}