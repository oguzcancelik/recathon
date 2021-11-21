using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Models.Requests.ServiceRequests;
using SpotifyGateway.Models.Responses.ServiceResponses;

namespace SpotifyGateway.ServiceClients.Abstractions
{
    public interface IPredictionServiceClient
    {
        Task<Dictionary<string, List<TrackModelResponse>>> GetModelResultsAsync(PredictionRequest request);
    }
}