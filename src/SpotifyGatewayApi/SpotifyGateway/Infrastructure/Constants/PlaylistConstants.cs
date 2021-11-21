using System.Collections.Immutable;
using SpotifyGateway.Models.Responses;
using SpotifyAPI.Web.Enums;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class PlaylistConstants
    {
        public const string SavedTracksId = "saved";
        public const string TopTracksId = "top";
        public const string RecentTracksId = "recent";

        public const string SavedTracksName = "Saved Tracks";
        public const string TopTracksName = "Top Tracks";
        public const string RecentTracksName = "Recent Tracks";

        public const string SavedTracksDescription = "tracks on your library.";
        public const string TopTracksDescription = "tracks you listened most on long, medium and short terms.";
        public const string RecentTracksDescription = "tracks from your recent listenin history.";

        public const string SpotifyLogoImagePath = "https://developer.spotify.com/assets/branding-guidelines/icon3@2x.png";

        public static readonly IImmutableList<TimeRangeType> TimeRanges = ImmutableList.Create(TimeRangeType.LongTerm, TimeRangeType.MediumTerm, TimeRangeType.ShortTerm);

        public static readonly IImmutableList<PlaylistResponse> DefaultPlaylists = ImmutableList.Create
        (
            new PlaylistResponse
            {
                Id = SavedTracksId,
                Name = SavedTracksName,
                Description = SavedTracksDescription,
                ImagePath = SpotifyLogoImagePath
            },
            new PlaylistResponse
            {
                Id = TopTracksId,
                Name = TopTracksName,
                Description = TopTracksDescription,
                ImagePath = SpotifyLogoImagePath
            },
            new PlaylistResponse
            {
                Id = RecentTracksId,
                Name = RecentTracksName,
                Description = RecentTracksDescription,
                ImagePath = SpotifyLogoImagePath
            }
        );
    }
}