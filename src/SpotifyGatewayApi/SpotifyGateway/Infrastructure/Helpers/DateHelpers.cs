using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class DateHelpers
    {
        public static DateTime GetNextClockTurn()
        {
            var currentDate = DateTime.UtcNow;

            return currentDate < currentDate.Date.AddHours(12)
                ? currentDate.Date.AddHours(12)
                : currentDate.Date.AddDays(1);
        }

        public static DateTime GetLastClockTurn()
        {
            var currentDate = DateTime.UtcNow;

            return currentDate < currentDate.Date.AddHours(12)
                ? currentDate.Date
                : currentDate.Date.AddHours(12);
        }

        public static TimeSpan GetTimeToClockTurn()
        {
            return GetNextClockTurn() - DateTime.UtcNow;
        }

        public static DateTime? GetAlbumReleaseDate(string releaseDate)
        {
            try
            {
                if (string.IsNullOrEmpty(releaseDate))
                {
                    return default;
                }

                foreach (var format in DateTimeConstants.AlbumReleaseDateFormats)
                {
                    var isParsed = releaseDate.TryParse(format, out var result);

                    if (isParsed)
                    {
                        return result;
                    }
                }

                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static IEnumerable<TimeSpan> SplitDay(int minutes)
        {
            return Enumerable.Range(0, 24 * 60 / minutes).Select(x => TimeSpan.FromMinutes(x * minutes));
        }
    }
}