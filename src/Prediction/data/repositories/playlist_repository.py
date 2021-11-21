from .base_repository import BaseRepository


class PlaylistRepository(BaseRepository):
    def __init__(self, engine):
        super().__init__(engine)

    def get_playlist_tracks(self, playlist_id: str):
        query = "select * from get_playlist_track_features(:playlist_id);"

        parameters = {
            "playlist_id": playlist_id
        }

        return super().query_all(query, parameters)

    def get_prediction_tracks(self, playlist_id: str, user_id: str, limit: int):
        query = "select * from get_prediction_tracks(:playlist_id, :user_id, :limit);"

        parameters = {
            "playlist_id": playlist_id,
            "user_id": user_id,
            "limit": limit
        }

        return super().query_all(query, parameters)

    def get_prediction_tracks_by_release_date(self, playlist_id: str):
        query = "select * from get_recently_released_tracks(:playlist_id);"

        parameters = {
            "playlist_id": playlist_id
        }

        return super().query_all(query, parameters)
