import re

from sqlalchemy import text


class BaseRepository:
    def __init__(self, engine):
        self.engine = engine

    def query_all(self, query: str, parameters: dict):
        with self.engine.connect() as connection:
            result = connection.execute(text(query), **parameters)
            result = [list(i) for i in result]

        return result

    @staticmethod
    def clear_query(query):
        query = query.replace("\n", "")
        query = re.sub(" {2,}", " ", query)
        return query
