using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WeatherHistoryApi.Utils;

public static class CacheHelper
{
    public static async Task<T> GetOrSetAsync<T>(
        IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        TimeSpan? absoluteExpiration = null,
        ILogger? logger = null)
    {
        // tenta pegar do cache
        var cached = await cache.GetStringAsync(key);
        if (cached != null)
        {
            logger?.LogInformation("Return cached data for {Key}", key);
            return JsonSerializer.Deserialize<T>(cached)!;
        }

        // se não existir no cache, gera o valor
        var result = await factory();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(5)
        };

        await cache.SetStringAsync(key, JsonSerializer.Serialize(result), options);
        logger?.LogInformation("Cached data for {Key}", key);

        return result;
    }
}
