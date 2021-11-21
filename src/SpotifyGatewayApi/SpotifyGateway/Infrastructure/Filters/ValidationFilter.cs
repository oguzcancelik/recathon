using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly ILoggerService _loggerService;

        public ValidationFilter(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                    .Select(x => new CustomError(x.ErrorMessage))
                    .ToList();

                var response = new BaseResponse<object>
                {
                    Errors = errors
                };

                await _loggerService.LogWarningAsync(errors, nameof(ValidationFilter), nameof(OnActionExecutionAsync));

                context.Result = new BadRequestObjectResult(response);

                return;
            }

            await next();
        }
    }
}