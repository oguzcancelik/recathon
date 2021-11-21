namespace SpotifyGateway.Infrastructure.Constants
{
    public static class DatabaseConstants
    {
        #region DatabaseNames

        public const string MongoDbDatabaseName = "Recathon";

        #endregion

        #region ColumnLengthLimits

        public const int ArtistNameLimit = 200;
        public const int AlbumNameLimit = 200;
        public const int TrackNameLimit = 200;
        public const int UserDisplayNameLimit = 50;
        public const int PlaylistNameLimit = 200;
        public const int PlaylistIdsLimit = 550;
        public const int ErrorMessageLimit = 200;

        #endregion

        #region Limits

        public const int RelatedArtistSearchReducedLimit = 3;
        public const int RelatedArtistSearchLimit = 10;
        public const int RelatedArtistSearchExtendedLimit = 20;

        public const int AlbumSearchReducedLimit = 5;
        public const int AlbumSearchLimit = 20;
        public const int AlbumSearchExtendedLimit = 40;

        public const int TrackSearchReducedLimit = 40;
        public const int TrackSearchLimit = 60;
        public const int TrackSearchExtendedLimit = 100;

        public const int MaxRecommendationLimit = 15;
        public const int MinRecommendationLimit = 7;

        public const int BulkInsertLimit = 250;

        #endregion

        #region DefaultFields

        public const string DefaultArtistName = "default_artist";
        public const string DefaultAlbumName = "default_album";
        public const string DefaultTrackName = "default_track";

        #endregion
    }
}