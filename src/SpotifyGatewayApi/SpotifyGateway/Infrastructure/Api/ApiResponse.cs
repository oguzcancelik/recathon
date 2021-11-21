using System.Net;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Infrastructure.Api
{
    public class ApiResponse<T>
    {
        public string Content { get; set; }

        public T Value { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public bool IsSuccessResponse => this.IsSuccessResponse();
    }
}