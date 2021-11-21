using System.Threading.Tasks;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class TokenHelpers
    {
        public static async Task<Token> GetTokenAsync(string clientId, string clientSecret)
        {
            var auth = new CredentialsAuth(clientId, clientSecret);

            return await auth.GetToken();
        }
    }
}