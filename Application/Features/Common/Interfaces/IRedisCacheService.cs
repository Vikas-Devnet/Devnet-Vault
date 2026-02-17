namespace Application.Features.Common.Interfaces;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string keyName, CancellationToken ctx = default);
    Task SetAsync<T>(string keyName, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null, CancellationToken ctx = default);
    Task RemoveAsync(string keyName, CancellationToken ctx = default);
}
