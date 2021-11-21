using System.Collections.Generic;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Cache;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class CacheMapper
    {
        public static CategoryCacheModel ToCacheModel(this List<CategoryResponse> responses)
        {
            return new CategoryCacheModel
            {
                Responses = responses,
                CreationTime = DateHelpers.GetLastClockTurn()
            };
        }
    }
}