using System.Collections.Generic;
using System.Net;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;

namespace SpotifyGateway.Models.Responses
{
    public static class DefaultResponses
    {
        public static readonly BaseResponse<object> BadRequestResponse = new()
        {
            Errors = new List<CustomError>
            {
                new(ErrorConstants.InvalidRequest, (int) HttpStatusCode.BadRequest, ErrorConstants.InvalidRequest)
            }
        };
    }
}