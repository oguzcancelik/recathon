using System;
using System.Collections.Generic;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Models.Cache
{
    public class CategoryCacheModel
    {
        public DateTime CreationTime { get; set; }

        public List<CategoryResponse> Responses { get; set; }
    }
}