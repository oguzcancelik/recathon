using SpotifyGateway.Models.Api.Abstractions;

namespace SpotifyGateway.Models.Api
{
    public class ApiAction : IApiAction
    {
        public string Path { get; set; }
    }
}