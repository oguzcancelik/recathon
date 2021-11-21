using System.Collections.Generic;

namespace SpotifyGateway.Models.Responses.ServiceResponses
{
    public class PredictionResponse
    {
        public Dictionary<string, List<TrackModelResponse>> Methods { get; set; }
    }
}