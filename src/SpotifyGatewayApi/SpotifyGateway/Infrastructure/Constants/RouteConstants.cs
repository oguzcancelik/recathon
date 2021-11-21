using System.Collections.Immutable;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class RouteConstants
    {
        #region Controllers

        public const string Auth = "auth";

        public const string Hangfire = "hangfire";

        public const string Playlists = "playlists";

        public const string Resources = "resources";

        public const string Users = "users";

        public const string Settings = "settings";

        public const string Swagger = "swagger";

        public const string Temp = "temp";

        public const string Tokens = "tokens";

        public const string Workers = "workers";

        #endregion

        #region Actions

        public const string Browse = "browse";

        public const string Callback = "mobile/callback";

        public const string Generate = "generate";

        public const string History = "history";

        public const string Info = "info";

        public const string PlaylistsAction = "playlists";

        public const string Recommend = "recommend";

        public const string Start = "start";

        public const string Stop = "stop";

        public const string Token = "token";

        #endregion

        public static readonly IImmutableList<string> ExactRoutes = ImmutableList.Create
        (
            $"/{Auth}/{Info}",
            $"/{Auth}/{Callback}",
            $"/{Playlists}/{Browse}",
            $"/{Playlists}/{Recommend}",
            $"/{Resources}",
            $"/{Settings}",
            $"/{Temp}",
            $"/{Tokens}/{Generate}",
            $"/{Users}/{PlaylistsAction}",
            $"/{Users}/{History}",
            $"/{Users}/{Token}",
            $"/{Workers}/{Start}",
            $"/{Workers}/{Stop}"
        );

        public static readonly IImmutableList<string> MatchingRoutes = ImmutableList.Create
        (
            Hangfire,
            Swagger
        );

        public static readonly IImmutableList<string> AdminRoutes = ImmutableList.Create
        (
            $"/{Resources}",
            $"/{Settings}",
            $"/{Users}/{Token}",
            $"/{Workers}/{Start}",
            $"/{Workers}/{Stop}"
        );

        public static readonly IImmutableList<string> SpotifyAuthenticateRoutes = ImmutableList.Create
        (
            $"/{Playlists}/{Recommend}",
            $"/{Users}/{PlaylistsAction}"
        );
    }
}