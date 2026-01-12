namespace BlogApp.Shared.Application.Caching;

public interface ICachingService
{
    /// <summary>
    /// Tries to get an item from the cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an item in the cache.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}