using System.Collections.Generic;

namespace SpotifyGateway.Models.Responses.Server.Auth
{
    public class AuthInfoResponse
    {
        public string ClientId { get; set; }

        public List<string> Scope { get; set; }

        public string RedirectUri { get; set; }
    }
}