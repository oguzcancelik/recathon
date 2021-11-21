using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Infrastructure.JsonConverters
{
    public class TrackConverter : JsonConverter<Track>
    {
        public override void WriteJson(JsonWriter writer, Track value, JsonSerializer serializer)
        {
            var values = new SortedDictionary<string, string>
            {
                {nameof(value.ArtistId), value.ArtistId}
            };

            serializer.Serialize(writer, values);
        }

        public override Track ReadJson(JsonReader reader, Type type, Track value, bool hasValue, JsonSerializer serializer)
        {
            return serializer.ReadJson(reader, type, value, hasValue);
        }
    }
}