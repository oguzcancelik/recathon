using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, T> RemoveSensitiveValues<T>(this Dictionary<string, T> values)
        {
            foreach (var key in MessageConstants.SensitiveKeys)
            {
                values.Remove(key);
            }

            return values;
        }

        public static bool IsDictionary(this object value)
        {
            var type = value.GetType();

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static Dictionary<string, object> ToDictionary(this object source, params string[] ignoredProperties)
        {
            var parameters = source
                .GetType()
                .GetProperties()
                .Where(x => !ignoredProperties.Contains(x.Name))
                .ToDictionary(x => x.Name, x => x.GetValue(source));

            return parameters;
        }
    }
}