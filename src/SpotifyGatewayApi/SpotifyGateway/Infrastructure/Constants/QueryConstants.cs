namespace SpotifyGateway.Infrastructure.Constants
{
    public static class QueryConstants
    {
        public const string DeletePlaylistTracksQuery = "DELETE FROM playlist_track WHERE playlist_id = @PlaylistId;";

        public const string GetAlbumsForTrackSearchQuery = "SELECT * FROM get_albums_for_track_search(@PlaylistId, @AlbumTypes, @DefinedLimit);";

        public const string GetAlternativeTrackIdsQuery = "SELECT * FROM get_alternative_track_ids(@PlaylistId, @UserId);";

        public const string GetCrendentialsQuery = "SELECT * FROM credential WHERE is_active = TRUE;";

        public const string GetCrendentialsToUpdateQuery = "SELECT * FROM credential WHERE is_updated = TRUE;";

        public const string GetArtistsForAlbumSearchQuery = "SELECT * FROM get_artists_for_album_search(@PlaylistId, @DefinedLimit);";

        public const string GetArtistsForRelatedArtistsSearchQuery = "SELECT * FROM get_artists_for_related_artists_search(@PlaylistId, @DefinedLimit);";

        public const string GetLastRecommendedTrackIdsQuery = "SELECT * FROM get_last_recommended_track_ids(@PlaylistId, @UserId);";

        public const string GetMultipleTracksQuery = "SELECT id, name, artist_id, artist_name FROM track WHERE id = ANY(@Ids);";

        public const string GetPlaylistQuery = "SELECT * FROM playlist WHERE id = @Id;";

        public const string GetPlaylistIdByRandomQuery = "SELECT id AS PlaylistId FROM playlist where (current_date - last_updated::date) < 2 ORDER BY random() LIMIT 1;";

        public const string GetPlaylistTracksQuery = "SELECT * FROM playlist_track WHERE playlist_id = @PlaylistId;";

        public const string GetRecentlyReleasedAlbumsForTrackSearchQuery = "SELECT * FROM get_recently_released_albums_for_track_search(@PlaylistId, @DefinedLimit);";

        public const string GetResourcesQuery = "SELECT * FROM resource WHERE is_active IS TRUE {0};";

        public const string GetSavedRelatedArtistInformationQuery = "SELECT * FROM get_saved_related_artist_information(@PlaylistId);";

        public const string GetSavedAlbumInformationQuery = "SELECT * FROM get_saved_album_information(@PlaylistId);";

        public const string GetSavedTrackInformationQuery = "SELECT * FROM get_saved_track_information(@PlaylistId);";

        public const string GetUserQuery = "SELECT * FROM users WHERE id = @Id;";

        public const string GetUserSessionQuery = "SELECT * FROM user_session WHERE session_guid = @SessionGuid;";

        public const string GetUsersRecommendationHistoryQuery = "SELECT * FROM get_users_recommendation_history(@UserId);";

        public const string GetWorkerQuery = "SELECT * FROM worker WHERE id = @Id;";

        public const string GetWorkersToRunOnStartupQuery = "SELECT * FROM worker WHERE run_on_startup = true AND is_enabled = true;";

        public const string UpdateAlbumsQuery = "UPDATE album SET is_completed = true, update_time = @UpdateTime WHERE id = ANY(@AlbumIds);";

        public const string UpdateConfigurationSetValueQuery = "UPDATE configuration SET value = @Value, update_time = @UpdateTime WHERE name = @Name AND application = @Application;";

        public const string UpdateConfigurationsSetIsUpdatedQuery = "UPDATE configuration SET is_updated = false WHERE application = @Application AND is_updated = true AND name = ANY(@Names);";

        public const string UpdateCredentialsUsageCountQuery = "UPDATE credential SET usage_count = usage_count + @Count WHERE client_id = @ClientId;";

        public const string UpdateDeletedCrendentialQuery = "UPDATE credential SET is_deleted = true, update_time = @UpdateTime WHERE client_id = @ClientId;";

        public const string UpdatePlaylistReduceSearchQuery = "UPDATE playlist SET is_search_reduced = true WHERE id = @Id;";

        public const string UpdatePlaylistIncreaseRecommendationCountQuery = "UPDATE playlist SET recommendation_count = recommendation_count + 1 WHERE id = @Id;";

        public const string UpdateWorkerQuery = "UPDATE worker SET is_working = @IsWorking, is_enabled = @IsEnabled, update_time = @UpdateTime WHERE id = @Id;";

        public const string GetCredentialQuery = @"
            SELECT client_id, access_token, token_type
            FROM credential
            WHERE @Type = ANY (type)
              AND @UsageType = ANY (usage_type)
              AND is_active = TRUE
              AND is_deleted = FALSE
              AND EXTRACT(EPOCH FROM @CurrentTime - token_update_time) / 60 < @TimeoutLimit
            ORDER BY random()
            LIMIT 1;";

        public const string GetRecommendedTrackInformationQuery = @"
            SELECT t.id, t.name, t.artist_name, a.image_path
            FROM track AS t
                     JOIN album AS a ON t.album_id = a.id
            WHERE t.id = ANY(@TrackIds);";

        public const string GetUserSessionByUserId = @"
            SELECT session_guid
            FROM user_session
            WHERE user_id = @UserId
            ORDER BY id DESC
            LIMIT 1;";

        public const string InsertAlbumQuery = @"
            INSERT INTO album (id, name, artist_id, artist_name, release_date, number_of_tracks, is_completed, type, image_path, creation_time, update_time)
            VALUES (@Id, @Name, @ArtistId, @ArtistName, @ReleaseDate, @NumberOfTracks, @IsCompleted, @Type, @ImagePath, @CreationTime, @UpdateTime)
            ON CONFLICT (id) DO NOTHING;";

        public const string InsertArtistGenreQuery = @"
            INSERT INTO artist_genre (artist_id, artist_name, genres) VALUES (@ArtistId, @ArtistName, @Genres)
            ON CONFLICT (artist_id) DO NOTHING;";

        public const string InsertPlaylistTrackQuery = @"
            INSERT INTO playlist_track (id, playlist_id, track_id, name, artist_id, artist_name, duration, key, mode, time_signature,
                acousticness, danceability, energy, instrumentalness, liveness, loudness, speechiness, valence, tempo)
            VALUES (@Id, @PlaylistId, @TrackId, @Name, @ArtistId, @ArtistName, @Duration, @Key, @Mode, @TimeSignature, 
                @Acousticness, @Danceability, @Energy, @Instrumentalness, @Liveness, @Loudness, @Speechiness, @Valence, @Tempo)
            ON CONFLICT (id) DO NOTHING;";

        public const string InsertRecommendationHistoryQuery = @"
            INSERT INTO recommendation_history (user_id, playlist_id, playlist_name, generated_playlist_id, generated_playlist_name, recommended_track_count,
                is_succeed, error_message, playlist_type, playlist_source, generate_type, start_time, end_time, creation_time, update_time)
            VALUES (@UserId, @PlaylistId, @PlaylistName, @GeneratedPlaylistId, @GeneratedPlaylistName, @RecommendedTrackCount,
                @IsSucceed, @ErrorMessage, @PlaylistType, @PlaylistSource, @GenerateType, @StartTime, @EndTime, @CreationTime, @UpdateTime)
            RETURNING id;";

        public const string InsertRecommendedTrackQuery = @"
            INSERT INTO recommended_track (recommendation_history_id, track_id, creation_time)
            VALUES (@RecommendationHistoryId, @TrackId, @CreationTime);";

        public const string InsertRelatedArtistQuery = @"
            INSERT INTO related_artists (id, artist_id, artist_name, related_artist_id, related_artist_name) 
            VALUES (@Id, @ArtistId, @ArtistName, @RelatedArtistId, @RelatedArtistName) 
            ON CONFLICT (id) DO NOTHING;";

        public const string InsertTrackQuery = @"
            INSERT INTO Track (id, name, album_id, album_name, artist_id, artist_name, duration, key, mode, time_signature, acousticness,
                danceability, energy, instrumentalness, liveness, loudness, speechiness, valence, tempo, creation_time, update_time)
            VALUES (@Id, @Name, @AlbumId, @AlbumName, @ArtistId, @ArtistName, @Duration, @Key, @Mode, @TimeSignature, @Acousticness,
                @Danceability, @Energy, @Instrumentalness, @Liveness, @Loudness, @Speechiness, @Valence, @Tempo, @CreationTime, @UpdateTime)
            ON CONFLICT (id) DO NOTHING;";

        public const string InsertUserQuery = @"
            INSERT INTO users (id, display_name, access_token, refresh_token, client_id, token_type, expires_in, creation_time, update_time)
            VALUES (@Id, @DisplayName, @AccessToken, @RefreshToken, @ClientId, @TokenType, @ExpiresIn, @CreationTime, @UpdateTime)
            ON CONFLICT (id) DO NOTHING;";

        public const string InsertUserSessionQuery = @"
            INSERT INTO user_session (user_id, session_guid, creation_time, update_time)
            VALUES (@UserId, @SessionGuid, @CreationTime, @UpdateTime);";

        public const string UpdateCredentialQuery = @"
            UPDATE credential 
            SET access_token = @AccessToken, is_updated = FALSE, is_active = @IsActive, is_deleted = FALSE, token_update_time = @TokenUpdateTime, update_time = @UpdateTime 
            WHERE client_id = @ClientId;";

        public const string UpdateArtistQuery = @"
            UPDATE artist SET name = @Name, update_time = @UpdateTime,
                saved_album_count = @SavedAlbumCount, album_offset = @AlbumOffset, album_count = @AlbumCount,
                saved_single_count = @SavedSingleCount, single_offset = @SingleOffset, single_count = @SingleCount,
                saved_compilation_count = @SavedCompilationCount, compilation_offset = @CompilationOffset, compilation_count = @CompilationCount
            WHERE id = @Id;";

        public const string UpdateUserQuery = @"
            UPDATE users
            SET access_token = @AccessToken, refresh_token = @RefreshToken, client_id = @ClientId, display_name = @DisplayName, update_time = @UpdateTime
            WHERE id = @Id;";

        public const string UpdateUserSessionQuery = @"
            UPDATE user_session
            SET session_guid = @SessionGuid, update_time = @UpdateTime
            WHERE id = @Id;";

        public const string UpsertArtistQuery = @"
            INSERT INTO artist (id, name, saved_album_count, album_offset, album_count, saved_single_count, single_offset,
                single_count, saved_compilation_count, compilation_offset, compilation_count, image_path, creation_time, update_time)
            VALUES (@Id, @Name, @SavedAlbumCount, @AlbumOffset, @AlbumCount, @SavedSingleCount, @SingleOffset,
                @SingleCount, @SavedCompilationCount, @CompilationOffset, @CompilationCount, @ImagePath, @CreationTime, @UpdateTime)
            ON CONFLICT (id)
            DO UPDATE SET image_path = @ImagePath WHERE artist.image_path IS NULL;";

        public const string UpsertPlaylistQuery = @"
            INSERT INTO playlist (id, name, owner_id, owner_name, is_public, is_collaborative, recommendation_count, playlist_type, last_updated, is_search_reduced, creation_time)
            VALUES (@Id, @Name, @OwnerId, @OwnerName, @IsPublic, @IsCollaborative, @RecommendationCount, @PlaylistType, @LastUpdated, @IsSearchReduced, @CreationTime)
            ON CONFLICT (id)
            DO UPDATE SET name = @Name, last_updated = @LastUpdated, owner_name = @OwnerName,
                owner_id = @OwnerId, is_public = @IsPublic, is_collaborative = @IsCollaborative;";
    }
}