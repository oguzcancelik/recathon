using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.SavedData;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class PlaylistHelpers
    {
        public static PlaylistType GetPlaylistType(string playlistId)
        {
            return playlistId switch
            {
                PlaylistConstants.SavedTracksId => PlaylistType.Saved,
                PlaylistConstants.TopTracksId => PlaylistType.Top,
                PlaylistConstants.RecentTracksId => PlaylistType.Recent,
                _ => PlaylistType.Playlist
            };
        }

        public static List<int> CalculateIndexes(int savedTrackCount, PlaylistType type)
        {
            var indexes = new List<int>();

            var limit = type switch
            {
                PlaylistType.Saved => SpotifyApiConstants.UserSavedTracksLimit,
                PlaylistType.Playlist => SpotifyApiConstants.PlaylistTracksLimit,
                _ => SpotifyApiConstants.UserSavedTracksLimit
            };

            var total = type switch
            {
                PlaylistType.Saved => SpotifyApiConstants.UserSavedTracksTotalLimit,
                PlaylistType.Playlist => SpotifyApiConstants.PlaylistTrackNumberLimit,
                _ => SpotifyApiConstants.UserSavedTracksTotalLimit
            };

            if (savedTrackCount <= limit)
            {
                return indexes;
            }

            var ratio = (total - limit) / limit;
            var divide = (savedTrackCount - limit) / ratio;

            if (divide < limit)
            {
                var count = (int)Math.Ceiling(decimal.Divide(savedTrackCount - limit, limit));

                indexes = Enumerable.Range(1, count).Select(x => x * limit).ToList();

                return indexes;
            }

            var random = new Random();

            for (var i = 1; i <= ratio; i++)
            {
                var index = random.Next(limit, i * divide + 1);

                indexes.Add(index);

                limit += divide;
            }

            return indexes;
        }

        public static bool IsExpired(PlaylistType type, DateTime lastUpdated)
        {
            var hoursPassed = (DateTime.UtcNow - lastUpdated).TotalHours;

            return type switch
            {
                PlaylistType.Playlist => hoursPassed > (int)ExpiryHour.Playlist,
                PlaylistType.Saved => hoursPassed > (int)ExpiryHour.Saved,
                PlaylistType.Top => hoursPassed > (int)ExpiryHour.Top,
                PlaylistType.Recent => hoursPassed > (int)ExpiryHour.Recent,
                _ => true
            };
        }

        public static SearchRange CalculateSearchRangeForRelatedArtist(SavedRelatedArtistData savedRelatedArtistData, int playlistArtistCount)
        {
            var isSmallRange = savedRelatedArtistData.RelatedArtistCount >= 40
                               || decimal.Divide(savedRelatedArtistData.RelatedArtistCount, playlistArtistCount) > (decimal)0.8;

            var isDefaultRange = savedRelatedArtistData.RelatedArtistCount >= 20
                                 || decimal.Divide(savedRelatedArtistData.RelatedArtistCount, playlistArtistCount) > (decimal)0.6;

            return isSmallRange
                ? SearchRange.Small
                : isDefaultRange
                    ? SearchRange.Default
                    : SearchRange.Large;
        }

        public static SearchRange CalculateSearchRangeForAlbum(SavedAlbumData savedAlbumData, int playlistArtistCount)
        {
            var isSmallRange = savedAlbumData.AlbumExistsArtistCount >= 40
                               || decimal.Divide(savedAlbumData.AlbumExistsArtistCount, playlistArtistCount) > (decimal)0.8;

            var isDefaultRange = savedAlbumData.AlbumExistsArtistCount >= 20
                                 || decimal.Divide(savedAlbumData.AlbumExistsArtistCount, playlistArtistCount) > (decimal)0.6;

            return isSmallRange
                ? SearchRange.Small
                : isDefaultRange
                    ? SearchRange.Default
                    : SearchRange.Large;
        }

        public static SearchRange CalculateSearchRangeForTrack(SavedTrackData savedTrackData, int playlistArtistCount)
        {
            var isSmallRange = savedTrackData.ArtistCount >= 40
                               || decimal.Divide(savedTrackData.ArtistCount, playlistArtistCount) > (decimal)0.8
                               || savedTrackData.ArtistCount >= 20 && savedTrackData.TrackCount > 3000;

            var isDefaultRange = savedTrackData.ArtistCount >= 20
                                 || decimal.Divide(savedTrackData.ArtistCount, playlistArtistCount) > (decimal)0.6
                                 || savedTrackData.ArtistCount >= 15 && savedTrackData.TrackCount > 1500;

            return isSmallRange
                ? SearchRange.Small
                : isDefaultRange
                    ? SearchRange.Default
                    : SearchRange.Large;
        }
    }
}