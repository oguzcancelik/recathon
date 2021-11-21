using SpotifyAPI.Web;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Contexts
{
    public class SpotifyUserContext : SpotifyContext, ISpotifyUserContext
    {
        public string SessionGuid { get; private set; }

        public string UserId { get; private set; }

        public CredentialType CredentialType => CredentialType.Auth;

        public SpotifyWebAPI Api { get; private set; }

        public User User { get; private set; }

        public void Set(string sessionGuid = null, string userId = null, User user = null)
        {
            SetSessionGuid(sessionGuid);
            SetUserId(userId);
            SetUser(user);
        }

        public void SetSessionGuid(string sessionGuid)
        {
            SetValue(nameof(sessionGuid), !string.IsNullOrEmpty(sessionGuid), () => SessionGuid = sessionGuid);
        }

        public void SetUserId(string userId)
        {
            SetValue(nameof(userId), !string.IsNullOrEmpty(userId), () => UserId = userId);
        }

        public void SetUser(User user)
        {
            SetValue(
                nameof(user),
                user != null,
                () =>
                {
                    User = user;
                    Api = new SpotifyWebAPI
                    {
                        AccessToken = user?.AccessToken,
                        TokenType = user?.TokenType
                    };
                }
            );
        }
    }
}