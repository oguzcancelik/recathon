using System;
using System.Globalization;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool TryParse(this string source, string format, out DateTime result)
        {
            return DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}