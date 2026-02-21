using Application.Features.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Cache
{
    public class RedisCacheService(IDistributedCache _cache) : IRedisCacheService
    {

        public async Task<T?> GetAsync<T>(string keyName, CancellationToken ctx = default)
        {
            var cachedData = await _cache.GetAsync(keyName, ctx);
            if (cachedData == null)
                return default;
            return JsonSerializer.Deserialize<T?>(cachedData);

        }

        public async Task SetAsync<T>(string keyName, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null, CancellationToken ctx = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(5),
                SlidingExpiration = unusedExpireTime ?? TimeSpan.FromMinutes(2)
            };
            var jsonData = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cache.SetAsync(keyName, jsonData, options, ctx);
        }

        public async Task RemoveAsync(string keyName, CancellationToken ctx = default)
        {
            await _cache.RemoveAsync(keyName, ctx);
        }
    }
}
