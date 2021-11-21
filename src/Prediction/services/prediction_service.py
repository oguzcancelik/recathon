import copy

import pandas as pd
from flask import current_app as app
from sklearn.ensemble import IsolationForest
from sklearn.neighbors import LocalOutlierFactor
from sklearn.svm import OneClassSVM

from data.repositories.playlist_repository import PlaylistRepository
from infrastructure.constants.config_constants import ConfigConstants
from infrastructure.constants.db_constants import DbConstants
from infrastructure.constants.feature_constants import FeatureConstants
from infrastructure.constants.model_constants import ModelConstants
from models.requests.prediction_request import PredictionRequest
from models.track_result import TrackResult


class PredictionService:
    def __init__(self, request: PredictionRequest):
        repository = PlaylistRepository(app.config[ConfigConstants.DbEngine])

        tracks = repository.get_playlist_tracks(request.PlaylistId)

        track = tracks[0]
        max_loudness, min_loudness, max_tempo, min_tempo = track[11], track[11], track[14], track[14]

        for i in tracks:
            loudness = i[11]
            tempo = i[14]

            if max_loudness < loudness:
                max_loudness = loudness

            if min_loudness > loudness:
                min_loudness = loudness

            if max_tempo < tempo:
                max_tempo = tempo

            if min_tempo > tempo:
                min_tempo = tempo

        max_loudness, min_loudness, max_tempo, min_tempo = abs(max_loudness), abs(min_loudness), abs(max_tempo), abs(min_tempo)
        max_tempo = max_tempo if max_tempo > min_tempo else min_tempo
        max_loudness = max_loudness if max_loudness > min_loudness else min_loudness

        limit = DbConstants.PredictionLimit[request.TryCount] if request.TryCount in DbConstants.PredictionLimit else DbConstants.PredictionLimit[-1]

        predict_tracks = repository.get_prediction_tracks(request.PlaylistId, request.UserId, limit)

        self.train_df = pd.DataFrame.from_records(tracks, columns=FeatureConstants.Features, index=FeatureConstants.TrackId)
        self.predict_df = pd.DataFrame.from_records(predict_tracks, columns=FeatureConstants.Features, index=FeatureConstants.TrackId)

        self.train_df[FeatureConstants.Tempo] = self.train_df[FeatureConstants.Tempo] / max_tempo
        self.train_df[FeatureConstants.Loudness] = self.train_df[FeatureConstants.Loudness] / max_loudness
        self.predict_df[FeatureConstants.Tempo] = self.predict_df[FeatureConstants.Tempo] / max_tempo
        self.predict_df[FeatureConstants.Loudness] = self.predict_df[FeatureConstants.Loudness] / max_loudness

        self.x_train = self.train_df.loc[:, FeatureConstants.Acousticness:].values.astype('float')
        self.x_predict = self.predict_df.loc[:, FeatureConstants.Acousticness:].values.astype('float')

        self.enabled_methods: list[str] = app.config[ConfigConstants.Settings].EnabledMethods
        self.try_count = request.TryCount

    def __run_model(self, model) -> list[TrackResult]:
        model_name = type(model).__name__

        try:
            model_parameters = ModelConstants.SuccessfulModelParameters[model_name]
            contamination = ModelConstants.Contamination

            if self.try_count < 2 or contamination not in model_parameters:
                model.set_params(**model_parameters)
            else:
                parameters = copy.deepcopy(model_parameters)
                parameters[contamination] -= self.try_count * 0.1
                model.set_params(**parameters)

            model.fit(self.x_train)
        except (ValueError, Exception):
            model.set_params(**ModelConstants.DefaultModelParameters[model_name])
            model.fit(self.x_train)

        predictions = model.predict(self.x_predict)

        df = {
            FeatureConstants.TrackId: self.predict_df.index,
            FeatureConstants.ArtistId: self.predict_df[FeatureConstants.ArtistId].tolist(),
            'result': predictions
        }

        result_df = pd.DataFrame(df)

        track_results = [TrackResult(row[FeatureConstants.TrackId], row[FeatureConstants.ArtistId]) for index, row in result_df.iterrows() if row['result'] == 1]

        return track_results

    def run_models(self) -> dict[str, list[TrackResult]]:

        method_dispatcher = {
            "IsolationForest": self.isolation_forest,
            "OneClassSVM": self.one_class_svm,
            "LocalOutlierFactor": self.local_outlier_factor
        }

        return {key: value() for key, value in method_dispatcher.items() if key in self.enabled_methods}

    def isolation_forest(self) -> list[TrackResult]:
        model = IsolationForest()
        return self.__run_model(model)

    def one_class_svm(self) -> list[TrackResult]:
        model = OneClassSVM()
        return self.__run_model(model)

    def local_outlier_factor(self) -> list[TrackResult]:
        model = LocalOutlierFactor()
        return self.__run_model(model)
