using System;
using System.Linq;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class ErrorExtensions
    {
        public static string ToErrorMessage(this Exception exception)
        {
            return exception.GetType() == typeof(CustomException)
                ? string.Join(" , ", ((CustomException) exception).Errors.Select(x => x.ToErrorMessage()))
                : exception.Message;
        }

        public static string ToErrorMessage(this CustomError error)
        {
            var code = error.Code.HasValue ? $"Code: {error.Code.Value} | " : null;
            var errorMessage = $"ErrorMessage: {(!string.IsNullOrEmpty(error.Message) ? error.Message : ErrorConstants.UnexpectedResponseFromSpotifyApi)}";
            var userFriendlyMessage = !string.IsNullOrEmpty(error.UserFriendlyMessage) ? $" | UserFriendlyMessage: {error.UserFriendlyMessage}" : null;

            return $"{{{code}{errorMessage}{userFriendlyMessage}}}";
        }

        public static CustomError ToCustomError(this Error error, string userFriendlyMessage = null)
        {
            var message = error != null
                ? error.Message
                : ErrorConstants.UnexpectedResponseFromSpotifyApi;

            return new CustomError(message, error?.Status, userFriendlyMessage);
        }

        public static CustomError ToCustomError(this Token token, int? code = null, string userFriendlyMessage = null)
        {
            var message = token != null
                ? $"{nameof(token.Error)}: {token.Error} | {nameof(token.ErrorDescription)}: {token.ErrorDescription}"
                : ErrorConstants.UnexpectedResponseFromSpotifyApi;

            return new CustomError(message, code, userFriendlyMessage);
        }

        public static CustomError ToCustomError(this string errorMessage, int? code = null)
        {
            return new CustomError(errorMessage, code);
        }
    }
}