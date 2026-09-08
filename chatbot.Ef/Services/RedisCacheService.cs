using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using StackExchange.Redis;

namespace chatbot.Ef.Services
{
    public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key)
        {
            var db = redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (!value.HasValue)
                return default;
            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiration);
        }
    }
}
