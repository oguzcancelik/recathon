using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class QueryExtensions
    {
        #region ToSelectValue

        public static (string, SelectType, string) ToSelectValue(this string name)
        {
            return (name, SelectType.Field, null);
        }

        public static (string, SelectType, string) ToSelectValue(this (string, SelectType) source)
        {
            var (name, select) = source;

            return (name, select, null);
        }

        #endregion
    }
}