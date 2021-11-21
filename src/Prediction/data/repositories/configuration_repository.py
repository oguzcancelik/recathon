from .base_repository import BaseRepository


class ConfigurationRepository(BaseRepository):
    def __init__(self, engine):
        super().__init__(engine)

    def get_global_settings(self, application_name: str) -> str:
        query = "select value from configuration where name = 'Global' and application = :application_name;"

        parameters = {
            "application_name": application_name
        }

        result = super().query_all(query, parameters)

        return result[0][0]
