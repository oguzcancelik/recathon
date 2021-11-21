using SpotifyGateway.Models.Api;
using SpotifyGateway.Models.Api.Abstractions;

namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class PredictionServiceOptions : IApiOptions
    {
        public string Url { get; set; }

        public ApiAction PredictAction { get; set; }
    }
}