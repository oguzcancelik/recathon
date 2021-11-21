using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Models.Enums;
using StackExchange.Redis;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class CacheExtensions
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> KeyedSemaphore = new();

        public static async Task<T> GetAsync<T>(this ICacheProvider cacheProvider, string key, CacheTime cacheTime, Func<Task<T>> func)
        {
            return await cacheProvider.GetAsync(key, cacheTime.ToTimeSpan(), func);
        }

        public static async Task<T> GetAsync<T>(this ICacheProvider cacheProvider, string key, int cacheMinute, Func<Task<T>> func)
        {
            return await cacheProvider.GetAsync(key, TimeSpan.FromMinutes(cacheMinute), func);
        }

        public static async Task<T> GetAsync<T>(this ICacheProvider cacheProvider, string key, TimeSpan expireTime, Func<Task<T>> func)
        {
            var (keyExists, value) = await cacheProvider.GetIfKeyExists<T>(key);

            if (keyExists)
            {
                return value;
            }

            T result;
            var semaphore = KeyedSemaphore.GetOrAdd(key, _ => new SemaphoreSlim(1));
            await semaphore.WaitAsync();

            try
            {
                (keyExists, value) = await cacheProvider.GetIfKeyExists<T>(key);

                if (keyExists)
                {
                    return value;
                }
                else
                {
                    result = await func();

                    if (result != null && expireTime > TimeSpan.Zero)
                    {
                        await cacheProvider.SetAsync(key, result, expireTime);
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }

            KeyedSemaphore.TryRemove(key, out _);

            return result;
        }

        public static T FromJson<T>(this RedisValue source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(source);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static TimeSpan ToTimeSpan(this CacheTime cacheTime)
        {
            return TimeSpan.FromMinutes((int) cacheTime);
        }
    }
}