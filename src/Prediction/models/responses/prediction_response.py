from dataclasses import dataclass

from models.track_result import TrackResult


@dataclass
class PredictionResponse:
    methods: dict[str, list[TrackResult]]

    def __init__(self, methods=None):
        self.methods = methods
