from collections import namedtuple

from infrastructure.constants.config_constants import ConfigConstants


def from_json(type_name: type, value: dict):
    return namedtuple(type_name.__name__, value.keys())(*value.values())


def parse(value, type_name):
    try:
        if type_name not in ConfigConstants.DefinedTypes:
            return value

        type_value = ConfigConstants.DefinedTypes[type_name]

        if type_value == str:
            return value

        if type_value == int:
            return int(value)

        if type_value == list[str]:
            return value.split(',')

        if type_value == list[int]:
            return [int(i) for i in value.split(',')]

    except (ValueError, Exception):
        pass

    return value
