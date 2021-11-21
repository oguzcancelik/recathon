using System;
using System.Collections.Generic;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Exceptions
{
    public class CustomException : Exception
    {
        public List<CustomError> Errors { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public Dictionary<string, object> LogValues { get; }

        public CustomException(
            Error error,
            string userFriendlyMessage,
            string className,
            string methodName,
            Dictionary<string, object> logValues = null
        )
        {
            var customError = error.ToCustomError(userFriendlyMessage);
            customError.UserFriendlyMessage ??= ErrorConstants.UnexpectedError;

            Errors = new List<CustomError> {customError};

            ClassName = className;
            MethodName = methodName;
            LogValues = logValues;
        }

        public CustomException(
            string errorMessage,
            string className,
            string methodName,
            Dictionary<string, object> logValues = null
        )
        {
            Errors = new List<CustomError>
            {
                new(errorMessage, ErrorConstants.UnexpectedError)
            };

            ClassName = className;
            MethodName = methodName;
            LogValues = logValues;
        }

        public CustomException(
            string errorMessage,
            string userFriendlyMessage,
            string className,
            string methodName,
            Dictionary<string, object> logValues = null
        )
        {
            Errors = new List<CustomError>
            {
                new(errorMessage, userFriendlyMessage ?? ErrorConstants.UnexpectedError)
            };

            ClassName = className;
            MethodName = methodName;
            LogValues = logValues;
        }

        public CustomException(
            string errorMessage,
            int code,
            string userFriendlyMessage,
            string className,
            string methodName,
            Dictionary<string, object> logValues
        )
        {
            Errors = new List<CustomError>
            {
                new(errorMessage, code, userFriendlyMessage ?? ErrorConstants.UnexpectedError)
            };

            ClassName = className;
            MethodName = methodName;
            LogValues = logValues;
        }

        public CustomException(
            CustomError customError,
            string className,
            string methodName,
            Dictionary<string, object> logValues
        )
        {
            customError.UserFriendlyMessage ??= ErrorConstants.UnexpectedError;

            Errors = new List<CustomError> {customError};

            ClassName = className;
            MethodName = methodName;
            LogValues = logValues;
        }
    }
}