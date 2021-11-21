using System;

namespace SpotifyGateway.Models.Cache
{
    public class RedisResult<T>
    {
        public TimeSpan? Expiry { get; set; }

        public T Value { get; set; }
    }
}