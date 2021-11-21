using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyGateway.Infrastructure.Api;
using SpotifyGateway.Models.Api;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class RequestExtensions
    {
        public static ApiAction Format(this ApiAction apiAction, params object[] values)
        {
            return new ApiAction
            {
                Path = apiAction.Path.Format(values)
            };
        }

        public static bool IsSuccessResponse<T>(this ApiResponse<T> apiResponse)
        {
            return apiResponse.HttpStatusCode.IsSuccessResponse();
        }

        public static bool IsSuccessResponse(this HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode is >= 200 and <= 299;
        }

        public static HttpClient AddHeaders(this HttpClient httpClient, IDictionary<string, string> headers)
        {
            if (headers == null || !headers.Any())
            {
                return httpClient;
            }

            foreach (var header in headers)
            {
                httpClient.AddHeader(header);
            }

            return httpClient;
        }

        public static void AddHeader(this HttpClient httpClient, KeyValuePair<string, string> header)
        {
            var (key, value) = header;

            if (httpClient.DefaultRequestHeaders.Contains(key))
            {
                httpClient.DefaultRequestHeaders.Remove(key);
            }

            httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public static async Task<ApiResponse<T>> ToApiResponseAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = new ApiResponse<T>
            {
                Content = content,
                HttpStatusCode = response.StatusCode
            };

            if (apiResponse.HttpStatusCode.IsSuccessResponse() && content.TryGetFromJson<T>(out var value))
            {
                apiResponse.Value = value;
            }

            return apiResponse;
        }

        public static bool TryGetResponse<T, TResponse>(this ApiResponse<T> apiResponse, out TResponse result)
        {
            result = default;

            if (apiResponse.HttpStatusCode.IsSuccessResponse())
            {
                return false;
            }

            return string.IsNullOrEmpty(apiResponse.Content) || apiResponse.Content.TryGetFromJson(out result);
        }
    }
}