using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class AlbumMapper
    {
        public static List<Album> ToEntity(this IEnumerable<SimpleAlbum> simpleAlbums)
        {
            return simpleAlbums
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault().ToEntity())
                .GroupBy(x =>
                {
                    var words = x.Name.RemoveKeywords().Split();
                    return x.ArtistId + words.First() + words.Last();
                }, StringComparer.InvariantCultureIgnoreCase)
                .Select(x => x.OrderByDescending(y => y.NumberOfTracks).First())
                .ToList();
        }

        private static Album ToEntity(this SimpleAlbum simpleAlbum)
        {
            return new Album
            {
                Id = simpleAlbum.Id,
                ArtistId = simpleAlbum.Artists.FirstOrDefault()?.Id,
                ArtistName = simpleAlbum.Artists.FirstOrDefault()?.Name.Limit(DatabaseConstants.ArtistNameLimit) ?? DatabaseConstants.DefaultArtistName,
                ImagePath = ImageHelpers.GetImagePath(simpleAlbum.Images),
                IsCompleted = false,
                Name = simpleAlbum.Name?.Limit(DatabaseConstants.AlbumNameLimit) ?? DatabaseConstants.DefaultAlbumName,
                NumberOfTracks = simpleAlbum.TotalTracks,
                ReleaseDate = DateHelpers.GetAlbumReleaseDate(simpleAlbum.ReleaseDate),
                Type = simpleAlbum.AlbumType,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };
        }
    }
}