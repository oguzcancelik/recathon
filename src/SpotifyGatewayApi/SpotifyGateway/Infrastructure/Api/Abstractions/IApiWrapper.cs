using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Models.Api.Abstractions;

namespace SpotifyGateway.Infrastructure.Api.Abstractions
{
    public interface IApiWrapper<out T> where T : class, IApiOptions
    {
        ApiResponse<TResult> Get<TResult>(Func<T, IApiAction> method, IDictionary<string, string> headers = null);

        Task<ApiResponse<TResult>> GetAsync<TResult>(Func<T, IApiAction> method, IDictionary<string, string> headers = null);

        Task<ApiResponse<TResult>> PostAsync<TResult>(Func<T, IApiAction> method, object body = null, IDictionary<string, string> headers = null);
    }
}