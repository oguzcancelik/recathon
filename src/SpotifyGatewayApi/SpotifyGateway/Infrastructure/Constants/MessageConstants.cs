using System.Collections.Immutable;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class MessageConstants
    {
        public static readonly IImmutableList<string> SensitiveKeys = ImmutableList.Create(
            "UserId",
            "ClientId",
            "SessionGuid"
        );
    }
}