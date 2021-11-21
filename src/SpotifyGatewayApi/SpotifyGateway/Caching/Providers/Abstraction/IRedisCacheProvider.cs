using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Models.Cache;
using StackExchange.Redis;

namespace SpotifyGateway.Caching.Providers.Abstraction
{
    public interface IRedisCacheProvider : ICacheProvider
    {
        Task<RedisResult<T>> GetWithExpiryTimeAsync<T>(string key);

        Task SetKeyExpireAsync(string key, TimeSpan expireTime, int db);

        Task<List<T>> HashGetAllAsync<T>(string key);

        Task HashSetAsync(string key, string hashKey, object data, TimeSpan? expiryTime = null);

        Task HashSetManyAsync(string key, HashEntry[] hashEntries, TimeSpan? expiryTime = null);

        Task HashDeleteAsync(string key, string hashKey);

        Task PubAsync(string channel, object message);

        Task SubAsync(string channel, Action<RedisChannel, RedisValue> action);
    }
}