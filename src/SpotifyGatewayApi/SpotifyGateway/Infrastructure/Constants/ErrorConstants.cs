namespace SpotifyGateway.Infrastructure.Constants
{
    public static class ErrorConstants
    {
        public const string CredentialsAuthError = "Error at CredentialsAuth";
        public const string ClientIdNotFoundError = "ClientId Not Found";
        public const string AuthCredentialNotFoundError = "AuthCredential Not Found";
        public const string TrackConverterError = nameof(TrackConverterError);

        #region UserFriendlyMessages

        public const string AddTracksPlaylistError = "An error occured during adding tracks to created playlist.";
        public const string CreatePlaylistError = "An error occured during creating a new playlist.";
        public const string GetCategoriesError = "An error occured during getting the browse categories.";
        public const string NumberOfRecommendedTrackIsNotEnough = "Number of recommended tracks is not enough.";
        public const string PlaylistIdNotFound = "PlaylistId should be provided for recommendation.";
        public const string InvalidPlaylistId = "Invalid Playlist Id!";
        public const string InvalidRequest = "Invalid request!";
        public const string UnexpectedError = "An error occured.";
        public const string UnexpectedResponseFromSpotifyApi = "Unexpected response from Spotify Api.";
        public const string UserAuthenticationError = "An error occured during authenticating the user.";
        public const string AppAuthenticationError = "An error occured during connecting to Spotify.";
        public const string UserIsNotAvailable = "User is not available for recommendation.";
        public const string UserPlaylistError = "An error occured during getting the user's playlist.";
        public const string UserPlaylistsError = "An error occured during getting the user's playlists.";
        public const string UserRecentTracksError = "An error occured while getting the user's recently played tracks.";
        public const string UserSavedTracksError = "An error occured while getting the user's saved tracks.";
        public const string UserTopTracksError = "An error occured while getting the user's top tracks.";
        public const string TooManyRequest = "Too many request.";
        public const string TrackCountNotEnough = "Count of user's {0} tracks is not enough for recommendation.";

        #endregion
    }
}