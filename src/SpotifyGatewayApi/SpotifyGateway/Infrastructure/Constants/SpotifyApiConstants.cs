using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class SpotifyApiConstants
    {
        #region RequestLimits // max allowed item limit per request 

        public const int ArtistAlbumsLimit = 50;
        public const int AlbumTracksLimit = 20;
        public const int CategoryLimit = 50;
        public const int PlaylistTracksLimit = 100;
        public const int RemovePlaylistTracksLimit = 100;
        public const int TrackAudioFeaturesLimit = 100;
        public const int UserPlaylistsLimit = 50;
        public const int UserRecentTracksLimit = 50;
        public const int UserSavedTracksLimit = 50;
        public const int UserTopTracksLimit = 50;

        #endregion

        #region MaxLimits // max limits defined by the application

        public const int BrowsePlaylistsExpireTimeout = 5;
        public const int MaxPlaylistCount = 100;
        public const int PlaylistTrackNumberLimit = 300;
        public const int TokenTimeoutLimit = 58;
        public const int CategoryPlaylistRetryLimit = 3;
        public const int UserSavedTracksTotalLimit = 200;

        public static readonly TimeSpan RetryWaitTime = TimeSpan.FromSeconds(3);

        #endregion

        #region RequestFields // other fields that used in api requests

        public const AlbumType AlbumTypes = AlbumType.Album | AlbumType.Compilation | AlbumType.Single;

        public const string PlaylistTracksRequestedFields = "items(is_local, track(id, artists(id, name), album(id, name), name, duration_ms)), error";
        public const string PlaylistTrackIdsRequestedFields = "items(track(id))";
        public static readonly string PlaylistRequestedFields = $"collaborative, id, error, images, name, owner(id, display_name), public, tracks(total, {PlaylistTracksRequestedFields})";

        public const string CategoryCountryCode = "US";
        public const string CategoryLanguageCode = "en_US";

        #endregion

        #region AuthFields // fields that are used during authentication processes

        public const Scope Scopes = Scope.PlaylistReadPrivate | Scope.PlaylistModifyPrivate | Scope.UgcImageUpload | Scope.UserTopRead | Scope.UserReadRecentlyPlayed | Scope.UserLibraryRead;
        public static readonly List<string> ScopeList = Scopes.GetStringAttribute(",").Split(",").ToList();

        #endregion
    }
}