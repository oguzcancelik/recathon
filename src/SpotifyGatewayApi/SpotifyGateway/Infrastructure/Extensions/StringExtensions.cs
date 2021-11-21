using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToAlphanumeric(this string source)
        {
            return Regex.Replace(source, "[^A-Za-z0-9 ]", string.Empty);
        }

        public static List<string> ToUri(this IEnumerable<string> ids)
        {
            return ids.Select(x => x.ToUri()).ToList();
        }

        public static List<DeleteTrackUri> ToDeleteTrackUri(this IEnumerable<string> ids)
        {
            return ids.Select(x => x.ToDeleteTrackUri()).ToList();
        }

        public static DeleteTrackUri ToDeleteTrackUri(this string id)
        {
            return new DeleteTrackUri(id.ToUri());
        }

        public static string ToUri(this string id)
        {
            return $"spotify:track:{id}";
        }

        public static string RemoveLineBreaks(this string source)
        {
            source = Regex.Replace(source, @"\t|\n|\r", string.Empty);

            return Regex.Replace(source, " {2,}", " ");
        }

        public static string RemoveKeywords(this string source)
        {
            source = ApplicationConstants.KeywordsForAlbumName
                .Aggregate(source, (current, keyword) => Regex.Replace(current, keyword, string.Empty, RegexOptions.IgnoreCase));

            return source.RemoveInvalidChars().Trim();
        }

        public static string RemoveInvalidChars(this string source)
        {
            return Regex.Replace(source, @"(\+|""|&|'|\(|\)|\[|\]|<|>|#|-)", string.Empty);
        }

        public static string Limit(this string source, int maxLength = StringConstants.DefaultMaxTextLength)
        {
            return source[..Math.Min(source.Length, maxLength)];
        }

        public static string Format(this string source, string value)
        {
            return value != null
                ? string.Format(source, value)
                : default;
        }

        public static string Format(this string source, List<string> values)
        {
            return values is { Count: > 0 }
                ? string.Format(source, values.ToArray<object>())
                : source;
        }

        public static string Format(this string source, params object[] values)
        {
            return values is { Length: > 0 }
                ? string.Format(source, values)
                : source;
        }

        public static string UpperFirstLetter(this string source)
        {
            var letters = source.ToCharArray();

            letters[0] = char.ToUpper(letters[0]);

            return new string(letters);
        }

        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return source.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static string Join(this IEnumerable<string> source, string seperator = ",")
        {
            return string.Join(seperator, source);
        }

        public static string ToBase64String(this string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }

        public static string FromBase64String(this string source)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(source));
        }

        public static string ToJson(this object source, Formatting formatting = Formatting.None, bool allowNull = true)
        {
            var jsonSerializerSettings = allowNull ? StringConstants.JsonDefaultSettings : StringConstants.JsonIgnoreNull;

            return JsonConvert.SerializeObject(source, formatting, jsonSerializerSettings);
        }

        public static bool TryGetFromJson<T>(this string source, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(source);

                return true;
            }
            catch (Exception)
            {
                result = default;

                return false;
            }
        }

        public static T FromJson<T>(this string source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T JsonToBson<T>(this T source)
        {
            return BsonSerializer.Deserialize<T>(source.ToJson());
        }

        public static T ToFromJson<T>(this T source)
        {
            return source.ToJson().FromJson<T>();
        }

        public static string ToSnakeCase(this string source)
        {
            var result = source.Select((x, i) =>
                char.IsLetter(x) && char.IsUpper(x)
                    ? i == 0
                        ? char.ToLower(x).ToString()
                        : $"_{char.ToLower(x)}"
                    : x.ToString()
            );

            return string.Join(string.Empty, result);
        }
    }
}