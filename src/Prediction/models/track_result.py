from dataclasses import dataclass


@dataclass
class TrackResult:
    TrackId: str
    ArtistId: str

    def __init__(self, track_id, artist_id):
        self.TrackId = track_id
        self.ArtistId = artist_id
