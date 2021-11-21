using SpotifyGateway.Models.Enums;
using SpotifyAPI.Web.Auth;

namespace SpotifyGateway.Models.Requests
{
    public class SpotifyAuthorizationRequest : AuthorizationCode
    {
        public DeviceType DeviceType { get; set; }

        public string State { get; set; }

        public string ClientId => State;
    }
}