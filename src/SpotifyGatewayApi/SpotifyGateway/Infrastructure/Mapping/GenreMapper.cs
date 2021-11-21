using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class GenreMapper
    {
        public static IEnumerable<ArtistGenre> ToArtistGenre(this IEnumerable<FullArtist> artists)
        {
            return artists.Select(x => new ArtistGenre
            {
                ArtistId = x.Id,
                ArtistName = x.Name,
                Genres = x.Genres
            });
        }
    }
}