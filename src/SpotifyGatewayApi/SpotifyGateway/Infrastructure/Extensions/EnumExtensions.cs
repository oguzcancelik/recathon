using System;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static bool TryParse<T>(this string source, out T value) where T : struct, Enum
        {
            return Enum.TryParse(source, true, out value);
        }

        public static T Parse<T>(this string source) where T : struct, Enum
        {
            return (T)Enum.Parse(typeof(T), source, false);
        }

        public static string ToValue(this SortType sortType)
        {
            return sortType switch
            {
                SortType.Asc => "ASC",
                SortType.Desc => "DESC",
                SortType.Random => "random()",
                _ => null
            };
        }
    }
}