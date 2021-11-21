using System;
using System.Threading.Tasks;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Caching.Providers
{
    public abstract class CacheProvider : ICacheProvider
    {
        public abstract Task<T> GetAsync<T>(string key);

        public abstract Task<T> GetSetAsync<T>(string key, object data, TimeSpan expiryTime);

        public abstract Task SetAsync(string key, object data, TimeSpan? expiryTime = null);

        public abstract Task InsertKeyAsync(string key, TimeSpan expiryTime);

        public abstract Task<(bool KeyExists, T Value)> GetIfKeyExists<T>(string key);

        public abstract Task<bool> KeyExistsAsync(string key);

        public abstract Task<bool> KeyDeleteAsync(string key);

        public abstract Task PushAsync(string key, string value);

        public abstract Task<T> PopAsync<T>(string key);

        public abstract Task<bool> CheckExistsAndDeleteAsync(string key);

        public abstract bool Lock(string key, TimeSpan expiryTime, string value = RedisConstants.Value);

        public abstract Task<bool> LockAsync(string key, TimeSpan expiryTime, string value = RedisConstants.Value);

        public abstract Task UnlockAsync(string key, string value = RedisConstants.Value);
    }
}