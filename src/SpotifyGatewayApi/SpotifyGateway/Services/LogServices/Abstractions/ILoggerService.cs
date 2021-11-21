using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Services.LogServices.Abstractions
{
    public interface ILoggerService
    {
        private const string ClassName = nameof(LoggerService);
        private const string ErrorMethod = nameof(LogErrorAsync);
        private const string WarningMethod = nameof(LogWarningAsync);

        Task LogErrorAsync(Token token, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null, int? code = null);

        Task LogErrorAsync(Error error, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null);

        Task LogErrorAsync(List<CustomError> errors, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null, string stackTrace = null);

        Task LogErrorAsync(string errorMessage, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null, string stackTrace = null, int? code = null);

        Task LogErrorAsync(CustomError error, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null, string stackTrace = null);

        Task LogWarningAsync(List<CustomError> errors, string className = ClassName, string methodName = ErrorMethod, Dictionary<string, object> logValues = null);

        Task LogWarningAsync(string errorMessage, string className = ClassName, string methodName = WarningMethod, Dictionary<string, object> logValues = null);

        Task LogWarningAsync(CustomError error, string className = ClassName, string methodName = WarningMethod, Dictionary<string, object> logValues = null);
    }
}