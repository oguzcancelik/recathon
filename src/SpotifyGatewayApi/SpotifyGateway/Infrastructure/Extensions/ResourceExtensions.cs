using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class ResourceExtensions
    {
        public static T SetValues<T>(this T resource, List<Resource> resources) where T : IResources
        {
            var properties = resource.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                var configuration = resources.FirstOrDefault(x => x.Name.EqualsIgnoreCase(property.Name));

                if (configuration != default && configuration.Value.TryConvert(property.PropertyType, out var result))
                {
                    property.SetValue(resource, result, null);
                }
            }

            return resource;
        }
    }
}