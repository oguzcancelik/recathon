using SpotifyAPI.Web;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Contexts.Abstractions
{
    public interface ISpotifyUserContext
    {
        string SessionGuid { get; }

        string UserId { get; }

        CredentialType CredentialType { get; }

        User User { get; }

        SpotifyWebAPI Api { get; }

        void Set(string sessionGuid = null, string userId = null, User user = null);

        void SetSessionGuid(string sessionGuid);

        void SetUserId(string userId);

        void SetUser(User user);
    }
}