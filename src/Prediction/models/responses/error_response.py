from dataclasses import dataclass


@dataclass
class ErrorResponse:
    errorMessage: str

    def __init__(self, errorMessage=None):
        self.errorMessage = errorMessage
