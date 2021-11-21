using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Cache;
using StackExchange.Redis;

namespace SpotifyGateway.Caching.Providers
{
    public class RedisCacheProvider : CacheProvider, IRedisCacheProvider
    {
        private readonly Dictionary<int, IDatabase> _databases = new();
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisOptions _redisOptions;

        public RedisCacheProvider(
            IConnectionMultiplexer connectionMultiplexer,
            IOptions<RedisOptions> redisOptions
        )
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisOptions = redisOptions.Value;
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            var value = await GetDatabase().StringGetAsync(key);

            return value.IsNullOrEmpty ? default : value.FromJson<T>();
        }

        public async Task<RedisResult<T>> GetWithExpiryTimeAsync<T>(string key)
        {
            var value = await GetDatabase().StringGetWithExpiryAsync(key);

            var redisResult = value.Value.IsNullOrEmpty
                ? default
                : new RedisResult<T>
                {
                    Expiry = value.Expiry,
                    Value = value.Value.FromJson<T>()
                };

            return redisResult;
        }

        public override async Task<T> GetSetAsync<T>(string key, object data, TimeSpan expiryTime)
        {
            var value = await GetAsync<T>(key);

            await SetAsync(key, data, expiryTime);

            return value;
        }

        public override async Task SetAsync(string key, object data, TimeSpan? expiryTime = null)
        {
            var value = data.ToJson();

            await GetDatabase().StringSetAsync(key, value, expiryTime);
        }

        public override async Task InsertKeyAsync(string key, TimeSpan expiryTime)
        {
            await SetAsync(key, RedisConstants.Value, expiryTime);
        }

        public override async Task<(bool KeyExists, T Value)> GetIfKeyExists<T>(string key)
        {
            var keyExists = await KeyExistsAsync(key);

            if (!keyExists)
            {
                return (false, default);
            }

            var result = await GetAsync<T>(key);

            return (true, result);
        }

        public override async Task<bool> KeyExistsAsync(string key)
        {
            return await GetDatabase().KeyExistsAsync(new RedisKey(key));
        }

        public override async Task<bool> KeyDeleteAsync(string key)
        {
            return await GetDatabase().KeyDeleteAsync(new RedisKey(key));
        }

        public override async Task PushAsync(string key, string value)
        {
            await GetDatabase(_redisOptions.QueueDb).ListRightPushAsync(new RedisKey(key), new RedisValue(value));
        }

        public override async Task<T> PopAsync<T>(string key)
        {
            var value = await GetDatabase(_redisOptions.QueueDb).ListLeftPopAsync(new RedisKey(key));

            return value.FromJson<T>();
        }

        public override async Task<bool> CheckExistsAndDeleteAsync(string key)
        {
            var keyExists = await KeyExistsAsync(key);

            if (keyExists)
            {
                await KeyDeleteAsync(key);
            }

            return keyExists;
        }

        public override bool Lock(string key, TimeSpan expiryTime, string value = RedisConstants.Value)
        {
            return GetDatabase(_redisOptions.LockDb).LockTake(key, value, expiryTime);
        }

        public override async Task<bool> LockAsync(string key, TimeSpan expiryTime, string value = RedisConstants.Value)
        {
            return await GetDatabase(_redisOptions.LockDb).LockTakeAsync(key, value, expiryTime);
        }

        public override async Task UnlockAsync(string key, string value = RedisConstants.Value)
        {
            await GetDatabase(_redisOptions.LockDb).LockReleaseAsync(key, value);
        }

        public async Task SetKeyExpireAsync(string key, TimeSpan expireTime, int db)
        {
            await GetDatabase(db).KeyExpireAsync(key, expireTime);
        }

        public async Task<List<T>> HashGetAllAsync<T>(string key)
        {
            var data = await GetDatabase(_redisOptions.HashDb).HashGetAllAsync(key);

            return data.Select(x => x.Value.FromJson<T>()).ToList();
        }

        public async Task HashSetAsync(string key, string hashKey, object data, TimeSpan? expiryTime = null)
        {
            var redisValue = new RedisValue(data.ToJson());

            await GetDatabase(_redisOptions.HashDb).HashSetAsync(key, hashKey, redisValue);

            if (expiryTime.HasValue)
            {
                await SetKeyExpireAsync(key, expiryTime.Value, _redisOptions.HashDb);
            }
        }

        public async Task HashSetManyAsync(string key, HashEntry[] hashEntries, TimeSpan? expiryTime = null)
        {
            await GetDatabase(_redisOptions.HashDb).HashSetAsync(key, hashEntries);

            if (expiryTime.HasValue)
            {
                await SetKeyExpireAsync(key, expiryTime.Value, _redisOptions.HashDb);
            }
        }

        public async Task HashDeleteAsync(string key, string hashKey)
        {
            await GetDatabase(_redisOptions.HashDb).HashDeleteAsync(key, hashKey);
        }

        public async Task PubAsync(string channel, object message)
        {
            await _connectionMultiplexer.GetSubscriber().PublishAsync(channel, message.ToJson());
        }

        public async Task SubAsync(string channel, Action<RedisChannel, RedisValue> action)
        {
            await _connectionMultiplexer.GetSubscriber().SubscribeAsync(channel, action);
        }

        private IDatabase GetDatabase(int db = -1)
        {
            if (_databases.TryGetValue(db, out var database))
            {
                return database;
            }

            database = _connectionMultiplexer.GetDatabase(db);

            _databases[db] = database;

            return database;
        }
    }
}