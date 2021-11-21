using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DevelopmentOnlyAttribute : Attribute, IActionFilter
    {
        private readonly IHostEnvironment _hostEnvironment;

        public DevelopmentOnlyAttribute(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_hostEnvironment.IsDevelopment())
            {
                context.Result = new BadRequestObjectResult(DefaultResponses.BadRequestResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}