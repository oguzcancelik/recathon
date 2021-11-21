using System;
using System.Threading.Tasks;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Caching.Providers.Abstraction
{
    public interface ICacheProvider
    {
        Task<T> GetAsync<T>(string key);

        Task<T> GetSetAsync<T>(string key, object data, TimeSpan expiryTime);

        Task SetAsync(string key, object data, TimeSpan? expiryTime = null);

        Task InsertKeyAsync(string key, TimeSpan expiryTime);

        Task<(bool KeyExists, T Value)> GetIfKeyExists<T>(string key);

        Task<bool> KeyExistsAsync(string key);

        Task<bool> KeyDeleteAsync(string key);

        Task PushAsync(string key, string value);

        Task<T> PopAsync<T>(string key);

        Task<bool> CheckExistsAndDeleteAsync(string key);

        bool Lock(string key, TimeSpan expiryTime, string value = RedisConstants.Value);

        Task<bool> LockAsync(string key, TimeSpan expiryTime, string value = RedisConstants.Value);

        Task UnlockAsync(string key, string value = RedisConstants.Value);
    }
}