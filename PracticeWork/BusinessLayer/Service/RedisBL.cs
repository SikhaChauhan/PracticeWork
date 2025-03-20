using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class RedisBL
    {
        private readonly IDatabase _cache;

        public RedisBL(IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            _cache = redis.GetDatabase();
        }

        // Store data in Redis with expiration time
        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _cache.StringSetAsync(key, value, expiry);
        }

        // Retrieve data from Redis
        public async Task<string?> GetAsync(string key)
        {
            return await _cache.StringGetAsync(key);
        }

        // Delete key from Redis
        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}
