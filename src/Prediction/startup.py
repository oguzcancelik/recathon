import json
import os

from sqlalchemy import create_engine

from data.repositories.configuration_repository import ConfigurationRepository
from infrastructure.configurations.settings import Settings
from infrastructure.constants.config_constants import ConfigConstants
from infrastructure.extensions import from_json


def get_configurations():
    environment = os.getenv("FLASK_ENV", "production")

    with open("settings.json") as file:
        settings = json.load(file)

    with open(f"settings.{environment}.json") as file:
        env_settings = json.load(file)

    return settings | env_settings


def get_dbengine(config):
    return create_engine(config[ConfigConstants.ConnectionString])


def get_settings(config):
    repository = ConfigurationRepository(config[ConfigConstants.DbEngine])

    result = repository.get_global_settings(config[ConfigConstants.Name])

    settings: Settings = from_json(Settings, json.loads(result))

    return settings
