using System.Collections.Generic;
using SpotifyGateway.Infrastructure.Exceptions;

namespace SpotifyGateway.Models.Responses
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Errors = new List<CustomError>();
        }

        public bool HasError => Errors.Count > 0;

        public List<CustomError> Errors { get; set; }

        public T Result { get; set; }
    }
}