using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class ArtistMapper
    {
        public static IEnumerable<Artist> ToEntity(this IEnumerable<FullArtist> artists)
        {
            return artists.Select(x => x.ToEntity());
        }

        public static Artist ToEntity(this FullArtist artist)
        {
            return new Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                ImagePath = ImageHelpers.GetImagePath(artist.Images, ImageResolution.Large),
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }
    }
}