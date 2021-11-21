using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Responses;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Middlewares
{
    public class SwaggerMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;

        public SwaggerMiddleware(IServiceProvider serviceProvider, RequestDelegate next)
        {
            _serviceProvider = serviceProvider;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authSettings = _serviceProvider.GetRequiredService<IAuthSettings>();

            var swaggerToken = context.Request.GetSwaggerToken();

            if (swaggerToken != authSettings.SwaggerToken)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = MimeTypes.JsonType;

                await context.Response.WriteAsync(DefaultResponses.BadRequestResponse.ToJson(), Encoding.UTF8);

                return;
            }

            await _next.Invoke(context);
        }
    }
}