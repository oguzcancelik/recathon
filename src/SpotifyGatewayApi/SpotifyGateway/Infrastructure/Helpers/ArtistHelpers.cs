using SpotifyGateway.Data.Entities;
using SpotifyAPI.Web.Enums;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class ArtistHelpers
    {
        public static AlbumType GetSearchAlbumType(Artist artist)
        {
            if (artist.AlbumCount == 0 || artist.AlbumCount != -1 && artist.AlbumOffset < artist.AlbumCount && artist.SavedAlbumCount < 100)
            {
                return AlbumType.Album;
            }

            if (artist.SingleCount == 0 || artist.SingleCount != -1 && artist.SingleOffset < artist.SingleCount && artist.SavedSingleCount < 100)
            {
                return AlbumType.Single;
            }

            if (artist.CompilationCount == 0 || artist.CompilationCount != -1 && artist.CompilationOffset < artist.CompilationCount && artist.SavedCompilationCount < 100)
            {
                return AlbumType.Compilation;
            }

            // returning All as default, which is filtered in the caller method
            return AlbumType.All;
        }
    }
}