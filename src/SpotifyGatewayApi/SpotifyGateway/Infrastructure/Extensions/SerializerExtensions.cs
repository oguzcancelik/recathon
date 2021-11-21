using System;
using Newtonsoft.Json;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class SerializerExtensions
    {
        public static T ReadJson<T>(this JsonSerializer serializer, JsonReader reader, Type type, T value, bool hasValue)
        {
            if (!hasValue)
            {
                value = (T) serializer.ContractResolver.ResolveContract(type).DefaultCreator?.Invoke();
            }

            if (value != null)
            {
                serializer.Populate(reader, value);

                return value;
            }

            throw new CustomException(ErrorConstants.TrackConverterError, nameof(SerializerExtensions), nameof(ReadJson));
        }
    }
}