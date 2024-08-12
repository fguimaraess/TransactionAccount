using Microsoft.Extensions.Caching.Memory;

namespace Domain.Interfaces.Data;

public interface ICacheService
{
    Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<ICacheEntry, Task<T>> factory);
}
