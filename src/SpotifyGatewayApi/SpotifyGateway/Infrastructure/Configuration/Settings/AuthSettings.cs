using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;

namespace SpotifyGateway.Infrastructure.Configuration.Settings
{
    public class AuthSettings : IAuthSettings
    {
        public string HangfireToken { get; set; }

        public string SwaggerToken { get; set; }

        public string AdminToken { get; set; }

        public string TokenKey { get; set; }

        public string SessionKey { get; set; }
    }
}