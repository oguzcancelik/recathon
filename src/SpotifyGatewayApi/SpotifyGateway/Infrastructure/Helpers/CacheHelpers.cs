using System;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class CacheHelpers
    {
        public static TimeSpan GetPlaylistExpiryTime(PlaylistType playlistType)
        {
            var hours = playlistType switch
            {
                PlaylistType.Playlist => (int) ExpiryHour.Playlist,
                PlaylistType.Saved => (int) ExpiryHour.Saved,
                //PlaylistType.Top => (int) ExpiryHour.Top, // ignored intentionally
                PlaylistType.Recent => (int) ExpiryHour.Recent,
                _ => (int) ExpiryHour.Playlist
            };

            return TimeSpan.FromHours(hours);
        }
    }
}