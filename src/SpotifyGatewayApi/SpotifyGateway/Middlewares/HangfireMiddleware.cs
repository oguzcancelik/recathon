using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Responses;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Middlewares
{
    public class HangfireMiddleware
    {
        private readonly RequestDelegate _next;

        public HangfireMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthSettings authSettings)
        {
            var hangfireToken = context.Request.GetHangfireToken();

            if (hangfireToken != authSettings.HangfireToken)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = MimeTypes.JsonType;

                await context.Response.WriteAsJsonAsync(DefaultResponses.BadRequestResponse);

                return;
            }

            await _next.Invoke(context);
        }
    }
}