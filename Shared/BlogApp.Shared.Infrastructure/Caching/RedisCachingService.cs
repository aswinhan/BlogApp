namespace BlogApp.Shared.Infrastructure.Caching;

public class RedisCachingService(
    IConnectionMultiplexer redis,
    ILogger<RedisCachingService> logger) : ICachingService
{
    private readonly IDatabase _database = redis.GetDatabase();
    // Default cache time if none provided (e.g., 10 minutes)
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var redisValue = await _database.StringGetAsync(key);
            if (redisValue.IsNullOrEmpty)
            {
                // Cache Miss
                return default;
            }

            // Cache Hit
            return JsonSerializer.Deserialize<T>(redisValue.ToString());
        }
        catch (Exception ex)
        {
            // If Redis is down, we don't want to crash the app. We just return null (Miss).
            logger.LogError(ex, "Redis error getting key {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var expiry = expiration ?? _defaultExpiration;
            var serializedValue = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(key, serializedValue, expiry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis error setting key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis error removing key {CacheKey}", key);
        }
    }
}