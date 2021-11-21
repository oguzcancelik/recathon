using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly.Retry;
using SpotifyGateway.Infrastructure.Api.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Requests.ServiceRequests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Models.Responses.ServiceResponses;
using SpotifyGateway.ServiceClients.Abstractions;

namespace SpotifyGateway.ServiceClients
{
    public class PredictionServiceClient : IPredictionServiceClient
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IApiWrapper<PredictionServiceOptions> _predictionApiWrapper;

        public PredictionServiceClient(
            AsyncRetryPolicy retryPolicy,
            IApiWrapper<PredictionServiceOptions> predictionApiWrapper
        )
        {
            _retryPolicy = retryPolicy;
            _predictionApiWrapper = predictionApiWrapper;
        }

        public async Task<Dictionary<string, List<TrackModelResponse>>> GetModelResultsAsync(PredictionRequest request)
        {
            try
            {
                var apiResponse = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var apiResponse = await _predictionApiWrapper.PostAsync<PredictionResponse>(x => x.PredictAction, request);

                    if (!apiResponse.IsSuccessResponse)
                    {
                        var errorMessage = apiResponse.TryGetResponse(out ErrorResponse errorResponse)
                            ? errorResponse.ErrorMessage
                            : ErrorConstants.UnexpectedError;

                        throw new Exception(errorMessage);
                    }

                    return apiResponse;
                });

                return apiResponse.Value.Methods;
            }
            catch (Exception e)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(request.PlaylistId), request.PlaylistId },
                    { nameof(request.UserId), request.UserId },
                    { nameof(request.GenerateType), request.GenerateType }
                };

                throw new CustomException(e.Message, ErrorConstants.UnexpectedError, nameof(PredictionServiceClient), nameof(GetModelResultsAsync), logValues);
            }
        }
    }
}