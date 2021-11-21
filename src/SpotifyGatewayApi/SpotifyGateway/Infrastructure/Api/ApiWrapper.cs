using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyGateway.Infrastructure.Api.Abstractions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Api.Abstractions;
using Unosquare.Labs.EmbedIO.Constants;

namespace SpotifyGateway.Infrastructure.Api
{
    public class ApiWrapper<T> : IApiWrapper<T> where T : class, IApiOptions
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly T _options;

        public ApiWrapper(
            IHttpClientFactory httpClientFactory,
            IOptions<T> options
        )
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public ApiResponse<TResponse> Get<TResponse>(Func<T, IApiAction> method, IDictionary<string, string> headers = null)
        {
            return GetAsync<TResponse>(method, headers).Result;
        }

        public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(Func<T, IApiAction> method, IDictionary<string, string> headers = null)
        {
            var apiMethod = method(_options);

            var endpoint = RequestHelpers.GenerateEndpoint(_options.Url, apiMethod.Path);

            var response = await CreateClient().AddHeaders(headers).GetAsync(endpoint);

            var apiResponse = await response.ToApiResponseAsync<TResponse>();

            return apiResponse;
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TResponse>(Func<T, IApiAction> method, object body = null, IDictionary<string, string> headers = null)
        {
            var apiMethod = method(_options);

            var endpoint = RequestHelpers.GenerateEndpoint(_options.Url, apiMethod.Path);

            var content = new StringContent(body.ToJson(), Encoding.UTF8, MimeTypes.JsonType);

            var response = await CreateClient().AddHeaders(headers).PostAsync(endpoint, content);

            var apiResponse = await response.ToApiResponseAsync<TResponse>();

            return apiResponse;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();

            return client;
        }
    }
}