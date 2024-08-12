using Domain.Interfaces.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Data;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<ICacheEntry, Task<T>> factory)
    {
        var result = await _cache.GetOrCreateAsync(cacheKey, factory);

        return result;
    }
}
