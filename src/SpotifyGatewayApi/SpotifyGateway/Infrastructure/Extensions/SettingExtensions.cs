using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class SettingExtensions
    {
        public static T SetValues<T>(this T settings, List<Data.Entities.Configuration> configurations) where T : ISettings
        {
            var properties = settings.GetType().GetProperties();
            var className = settings.GetType().Name;

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }

                var configuration = configurations.FirstOrDefault(x => className.ContainsIgnoreCase(x.Class) && x.Name.EqualsIgnoreCase(property.Name));

                if (configuration != default && configuration.Value.TryConvert(property.PropertyType, out var result))
                {
                    property.SetValue(settings, result, null);
                }
            }

            return settings;
        }

        public static void SetValue<T>(this T settings, string propertyName, object value) where T : ISettings
        {
            var property = settings.GetType().GetProperties().FirstOrDefault(x => x.Name == propertyName);

            if (property != default && property.CanRead && property.CanWrite && value.TryConvert(property.PropertyType, out var result))
            {
                property.SetValue(settings, result, null);
            }
        }

        public static Dictionary<string, object> GetValues(this ISettings settings)
        {
            var values = settings.ToJson().FromJson<Dictionary<string, object>>();

            var stringKeys = values
                .Where(x => x.Value is string)
                .Select(x => x.Key)
                .ToList();

            var dictionaryKeys = values
                .Where(x => x.Value is JObject)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in stringKeys)
            {
                var value = (string)values[key];

                values[key] = value.Mask();
            }

            foreach (var key in dictionaryKeys)
            {
                values[key] = null;
            }

            return values;
        }
    }
}