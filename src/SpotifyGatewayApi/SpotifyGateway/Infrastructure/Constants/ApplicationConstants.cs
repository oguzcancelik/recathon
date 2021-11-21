using System.Collections.Generic;
using System.Collections.Immutable;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class ApplicationConstants
    {
        public static readonly List<string> SpotifyPlaylistIds = new() { "spotify", "spotifycharts" };

        #region GeneratedFields

        public const string AppPrefix = "rec | ";

        #endregion

        #region Limits

        public const int TrackCountLimit = 30;
        public const int CategoryPlaylistLimit = 5;

        #endregion

        public static readonly IImmutableList<string> KeywordsForAlbumName = ImmutableList.Create
        (
            "International Version",
            "Platinum Edition",
            "Special Edition",
            "Deluxe Version",
            "Deluxe Edition",
            "Deluxe Audio",
            "Deluxe",
            "Expanded Edition",
            "Expanded",
            "Remastered",
            "featuring",
            "feat",
            "ft.",
            "ft"
        );
    }
}