using System.Collections.Generic;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Infrastructure.Factories.Abstractions
{
    public interface IResourceFactory<out T>
    {
        T Value { get; }
    }

    public interface IResourceFactory
    {
        List<IResources> GetResources(ResourcesClass resourcesClass);
    }
}