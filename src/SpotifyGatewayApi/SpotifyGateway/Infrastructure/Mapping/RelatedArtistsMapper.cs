using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class RelatedArtistsMapper
    {
        public static RelatedArtist ToEntity(this Artist artist)
        {
            return new RelatedArtist
            {
                Id = $"{artist.Id}{artist.Id}",
                ArtistId = artist.Id,
                ArtistName = artist.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                RelatedArtistId = artist.Id,
                RelatedArtistName = artist.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName
            };
        }

        public static IEnumerable<RelatedArtist> ToEntity(this IEnumerable<FullArtist> relatedArtists, Artist artist)
        {
            return relatedArtists.Select(x => x.ToEntity(artist)).ToList();
        }

        public static RelatedArtist ToEntity(this FullArtist relatedArtist, Artist artist)
        {
            return new RelatedArtist
            {
                Id = $"{artist.Id}{relatedArtist.Id}",
                ArtistId = artist.Id,
                ArtistName = artist.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                RelatedArtistId = relatedArtist.Id,
                RelatedArtistName = relatedArtist.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName
            };
        }
    }
}