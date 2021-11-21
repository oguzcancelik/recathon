using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Middlewares
{
    public class RequestInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items.Add(HttpConstants.RequestId, Guid.NewGuid().ToString());

            await _next.Invoke(context);
        }
    }
}