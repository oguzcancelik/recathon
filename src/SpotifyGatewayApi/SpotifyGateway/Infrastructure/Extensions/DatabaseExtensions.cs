using System;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static string ToLogCollection(this DateTime dateTime)
        {
            return $"{nameof(Log)}-{dateTime.ToCollectionString()}";
        }

        public static string ToSearchResultCollection(this DateTime dateTime)
        {
            return $"{nameof(SearchResult)}-{dateTime.ToCollectionString()}";
        }

        public static string ToCollectionString(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeConstants.MongoCollectionFormat);
        }
    }
}