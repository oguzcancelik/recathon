using System.Linq;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class RequestHelpers
    {
        public static bool IsAdminRoute(string path)
        {
            return RouteConstants.AdminRoutes.Any(path.EqualsIgnoreCase);
        }

        public static bool ShortcutRequest(string path)
        {
            return !RouteConstants.ExactRoutes.Any(path.EqualsIgnoreCase)
                   && !RouteConstants.MatchingRoutes.Any(path.ContainsIgnoreCase);
        }

        public static string GenerateEndpoint(string url, string path)
        {
            return $"{url}{path}";
        }
    }
}