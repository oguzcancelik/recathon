namespace SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction
{
    public interface IAuthSettings : ISettings
    {
        string HangfireToken { get; }

        string SwaggerToken { get; }

        string AdminToken { get; }

        string TokenKey { get; }

        string SessionKey { get; }
    }
}