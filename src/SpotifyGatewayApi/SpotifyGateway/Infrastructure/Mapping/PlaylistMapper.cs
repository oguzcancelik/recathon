using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class PlaylistMapper
    {
        public static Playlist ToEntity(this FullPlaylist fullPlaylist)
        {
            return new Playlist
            {
                Id = fullPlaylist.Id,
                IsCollaborative = fullPlaylist.Collaborative,
                IsPublic = fullPlaylist.Public || ApplicationConstants.SpotifyPlaylistIds.Contains(fullPlaylist.Id),
                IsSearchReduced = false,
                LastUpdated = DateTime.UtcNow,
                Name = fullPlaylist.Name.Limit(DatabaseConstants.PlaylistNameLimit),
                OwnerId = fullPlaylist.Owner.Id,
                OwnerName = fullPlaylist.Owner.DisplayName,
                PlaylistType = PlaylistType.Playlist,
                RecommendationCount = default,
                Tracks = new List<Track>(),
                CreationTime = DateTime.UtcNow
            };
        }

        public static List<PlaylistResponse> ToResponse(this IEnumerable<SimplePlaylist> simplePlaylists)
        {
            return simplePlaylists.Select(x => x.ToResponse()).ToList();
        }

        public static PlaylistResponse ToResponse(this SimplePlaylist simplePlaylist)
        {
            var owner = !string.IsNullOrEmpty(simplePlaylist.Owner.DisplayName)
                ? simplePlaylist.Owner.DisplayName
                : simplePlaylist.Owner.Id;

            return new PlaylistResponse
            {
                Id = simplePlaylist.Id,
                Description = $"a playlist by {owner}.",
                ImagePath = ImageHelpers.GetImagePath(simplePlaylist.Images),
                IsAd = false,
                IsPublic = simplePlaylist.Public,
                Name = simplePlaylist.Name,
                TrackCount = simplePlaylist.Tracks.Total
            };
        }

        public static GeneratedPlaylistResponse ToResponse(this FullPlaylist fullPlaylist)
        {
            return new GeneratedPlaylistResponse
            {
                Id = fullPlaylist.Id,
                ImagePath = ImageHelpers.GetImagePath(fullPlaylist.Images),
                IsAd = false,
                IsPublic = fullPlaylist.Public,
                Name = fullPlaylist.Name,
                TrackCount = fullPlaylist.Tracks.Total
            };
        }

        public static GeneratedPlaylistResponse ToResponse(this PlaylistResponse playlistResponse)
        {
            return new GeneratedPlaylistResponse
            {
                Id = playlistResponse.Id,
                Description = playlistResponse.Description,
                ImagePath = playlistResponse.ImagePath,
                IsAd = false,
                IsPublic = playlistResponse.IsPublic,
                Name = playlistResponse.Name,
                TrackCount = playlistResponse.TrackCount
            };
        }

        public static List<CategoryPlaylistResponse> ToResponse(this IEnumerable<PlaylistResponse> playlistResponses, string categoryId)
        {
            return playlistResponses.Select(x => x.ToResponse(categoryId)).ToList();
        }

        public static CategoryPlaylistResponse ToResponse(this PlaylistResponse playlistResponse, string categoryId)
        {
            return new CategoryPlaylistResponse
            {
                Id = playlistResponse.Id,
                CategoryId = categoryId,
                Description = playlistResponse.Description,
                ImagePath = playlistResponse.ImagePath,
                IsAd = false,
                IsPublic = playlistResponse.IsPublic,
                Name = playlistResponse.Name,
                TrackCount = playlistResponse.TrackCount
            };
        }

        public static Playlist Merge(this Playlist oldPlaylist, Playlist newPlaylist)
        {
            newPlaylist.RecommendationCount = oldPlaylist.RecommendationCount;
            newPlaylist.CreationTime = oldPlaylist.CreationTime;

            // recalculate saved information on new playlist fetching
            // newPlaylist.IsSearchReduced = oldPlaylist.IsSearchReduced;

            return newPlaylist;
        }
    }
}