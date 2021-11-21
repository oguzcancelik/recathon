using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.QueryParameters;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SearchType = SpotifyGateway.Models.Enums.SearchType;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class ParameterGenerator
    {
        public static SearchParameters SearchParameters(string playlistId, SearchType type, SearchRange searchRange = SearchRange.Default, bool includeAllAlbums = false)
        {
            return new SearchParameters
            {
                PlaylistId = playlistId,
                DefinedLimit = GetDefinedLimit(type, searchRange),
                AlbumTypes = GetAlbumTypes(type, includeAllAlbums)
            };
        }

        private static int GetDefinedLimit(SearchType type, SearchRange searchRange)
        {
            return type switch
            {
                SearchType.RelatedArtists => searchRange switch
                {
                    SearchRange.Default => DatabaseConstants.RelatedArtistSearchLimit,
                    SearchRange.Small => DatabaseConstants.RelatedArtistSearchReducedLimit,
                    SearchRange.Large => DatabaseConstants.RelatedArtistSearchExtendedLimit,
                    _ => default
                },
                SearchType.Album => searchRange switch
                {
                    SearchRange.Default => DatabaseConstants.AlbumSearchLimit,
                    SearchRange.Small => DatabaseConstants.AlbumSearchReducedLimit,
                    SearchRange.Large => DatabaseConstants.AlbumSearchExtendedLimit,
                    _ => default
                },
                SearchType.Track => searchRange switch
                {
                    SearchRange.Default => DatabaseConstants.TrackSearchLimit,
                    SearchRange.Small => DatabaseConstants.TrackSearchReducedLimit,
                    SearchRange.Large => DatabaseConstants.TrackSearchExtendedLimit,
                    _ => default
                },
                _ => default
            };
        }

        private static string GetAlbumTypes(SearchType type, bool includeAllAlbums)
        {
            return type switch
            {
                SearchType.RelatedArtists => null,
                SearchType.Album => null,
                SearchType.Track => includeAllAlbums
                    ? $"{{{AlbumType.All.GetStringAttribute()}}}"
                    : $"{{{AlbumType.Album.GetStringAttribute()}}}",
                _ => null
            };
        }
    }
}