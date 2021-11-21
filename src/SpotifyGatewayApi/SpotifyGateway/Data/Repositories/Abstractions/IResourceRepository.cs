using System.Collections.Generic;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.FilterModels;

namespace SpotifyGateway.Data.Repositories.Abstractions
{
    public interface IResourceRepository
    {
        List<Resource> GetResources(ResourceFilterModel resourceFilterModel = null);
    }
}