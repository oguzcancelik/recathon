using SpotifyAPI.Web;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Contexts.Abstractions
{
    public interface ISpotifyAppContext
    {
        string ClientId { get; }

        SpotifyWebAPI Api { get; }

        CredentialType CredentialType { get; }

        void Set(string clientId, string accessToken, string tokenType);

        void SetClientId(string clientId);

        void SetApi(string accessToken, string tokenType);
    }
}