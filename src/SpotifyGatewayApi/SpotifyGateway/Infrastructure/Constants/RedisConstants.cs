using System;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class RedisConstants
    {
        #region CachePrefixes

        public const string AdFreeUser = "AdFreeUser:";
        public const string CategoryPlaylistsCache = "CategoryPlaylists";
        public const string InsufficientPlaylistCache = "InsufficientPlaylist:";
        public const string PlaylistCache = "Playlist:";
        public const string RecommendationHistory = "RecommendationHistory:";
        public const string SessionGuidCache = "SessionGuid:";
        public const string TokenCache = "Token:";
        public const string TokenRefreshTimeCache = "TokenRefreshTime";
        public const string CategoryRefreshCache = "CategoryRefresh";
        public const string UserCache = "User:";
        public const string UserPlaylistsCache = "UserPlaylists:";

        #endregion

        #region Locks

        public const string AlbumLock = "AlbumLock:";
        public const string ArtistLock = "ArtistLock:";
        public const string CategoryPlaylistsLock = "CategoryPlaylistsLock";
        public const string RelatedArtistsLock = "RelatedArtistsLock:";
        public const string UserLock = "UserLock:";
        public const string UserRecentTracksLock = "UserRecentTracksLock:";
        public const string UserTokenLock = "UserTokenLock:";
        public const string UserTopTracksLock = "UserTopTracksLock:";

        #endregion

        #region Queues

        public const string SearchQueue = nameof(SearchQueue);

        #endregion

        #region Values

        public const string EmptyCache = nameof(EmptyCache);
        public const string Locked = nameof(Locked);
        public const string Value = nameof(Value);

        #endregion

        #region ExpiryTimes

        public static readonly TimeSpan DefaultExpiryTime = TimeSpan.FromMinutes(1);

        public static readonly TimeSpan AdFreeUserKeyExpiryTime = TimeSpan.FromHours(4);
        public static readonly TimeSpan AuthExpiryTime = TimeSpan.FromHours(1);
        public static readonly TimeSpan CategoryRefreshKeyCacheExpiryTime = TimeSpan.FromHours(1);
        public static readonly TimeSpan CategoryPlaylistsCacheExpiryTime = TimeSpan.FromHours(50);
        public static readonly TimeSpan CategoryPlaylistsLockExpiryTime = TimeSpan.FromMinutes(2);
        public static readonly TimeSpan InsufficientPlaylistKeyExpiryTime = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan HistoryExpiryTime = TimeSpan.FromDays(10);
        public static readonly TimeSpan SearchLockExpiryTime = TimeSpan.FromMinutes(3);
        public static readonly TimeSpan TokenCacheExpiryTime = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan UserPlaylistsCacheExpiryTime = TimeSpan.FromSeconds(45);
        public static readonly TimeSpan UserRecentTracksLockExpiryTime = TimeSpan.FromHours(1);
        public static readonly TimeSpan UserTokenLockExpiryTime = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan UserTopTracksLockExpiryTime = TimeSpan.FromDays(1);

        #endregion

        #region Events

        public const string UpdateSettingEvent = nameof(UpdateSettingEvent);
        public const string UpdateSettingsEvent = nameof(UpdateSettingsEvent);
        public const string UpdateResourcesEvent = nameof(UpdateResourcesEvent);

        #endregion
    }
}