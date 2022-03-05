using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyGateway.Models.Responses;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Middlewares
{
    public class ShortcutMiddleware
    {
        public ShortcutMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            context.Response.ContentType = MimeTypes.JsonType;

            await context.Response.WriteAsJsonAsync(DefaultResponses.BadRequestResponse);
        }
    }
}