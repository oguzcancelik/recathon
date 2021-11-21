using System.Collections.Immutable;

namespace SpotifyGateway.Infrastructure.Constants
{
    public class DateTimeConstants
    {
        public static readonly IImmutableList<string> AlbumReleaseDateFormats = ImmutableList.Create(DayFormat, MonthFormat, YearFormat);

        public const string YearFormat = "yyyy";
        public const string MonthFormat = "yyyy-MM";
        public const string DayFormat = "yyyy-MM-dd";

        public const string MongoCollectionFormat = "yyyy.MM.dd";
    }
}