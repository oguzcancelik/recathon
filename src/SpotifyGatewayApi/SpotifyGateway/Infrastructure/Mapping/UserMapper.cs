using System;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class UserMapper
    {
        public static User ToEntity(this Token token, PrivateProfile profile, string clientId)
        {
            return new User
            {
                Id = profile.Id,
                AccessToken = token.AccessToken,
                DisplayName = profile.DisplayName.Limit(DatabaseConstants.UserDisplayNameLimit),
                ExpiresIn = token.ExpiresIn,
                RefreshToken = token.RefreshToken,
                ClientId = clientId,
                TokenType = token.TokenType,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }
    }
}