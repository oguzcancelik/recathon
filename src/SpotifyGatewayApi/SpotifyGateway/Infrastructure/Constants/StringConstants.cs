using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class StringConstants
    {
        public const int DefaultMaxTextLength = 100;
        public const string Null = "..null..";
        public const string EmptyString = "..empty string..";

        public static readonly JsonSerializerSettings JsonDefaultSettings = new()
        {
            Converters = new List<JsonConverter> {new StringEnumConverter()}
        };

        public static readonly JsonSerializerSettings JsonIgnoreNull = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> {new StringEnumConverter()}
        };
    }
}