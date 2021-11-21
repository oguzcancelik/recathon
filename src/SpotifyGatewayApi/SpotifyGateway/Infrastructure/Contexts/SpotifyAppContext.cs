using SpotifyAPI.Web;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Contexts
{
    public class SpotifyAppContext : SpotifyContext, ISpotifyAppContext
    {
        public SpotifyWebAPI Api { get; private set; }

        public string ClientId { get; private set; }

        public CredentialType CredentialType => CredentialType.App;

        public void Set(string clientId, string accessToken, string tokenType)
        {
            SetClientId(clientId);
            SetApi(accessToken, tokenType);
        }

        public void SetClientId(string clientId)
        {
            SetValue(nameof(clientId), !string.IsNullOrEmpty(clientId), () => ClientId = clientId);
        }

        public void SetApi(string accessToken, string tokenType)
        {
            SetValue(
                nameof(Api),
                !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(tokenType),
                () => Api = new SpotifyWebAPI
                {
                    AccessToken = accessToken,
                    TokenType = tokenType
                }
            );
        }
    }
}