namespace BlogApp.Shared.Infrastructure.Caching;

// This decorator wraps ANY QueryHandler where the Query implements ICacheableQuery
public sealed class CachingQueryHandler<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> decorated,
    ICachingService cache,
    ILogger<CachingQueryHandler<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>, ICacheableQuery // <--- Only applies to cacheable queries
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        // 1. Check Cache
        TResponse? cachedResult = await cache.GetAsync<TResponse>(query.CacheKey, cancellationToken);

        if (cachedResult is not null)
        {
            logger.LogDebug("Cache HIT for {QueryName} ({CacheKey})", typeof(TQuery).Name, query.CacheKey);
            return cachedResult; // Implicit conversion to Result<T>
        }

        // 2. Cache Miss - Call the real handler (Database)
        logger.LogDebug("Cache MISS for {QueryName} ({CacheKey})", typeof(TQuery).Name, query.CacheKey);

        Result<TResponse> result = await decorated.Handle(query, cancellationToken);

        // 3. Save to Cache (only if successful)
        if (result.IsSuccess)
        {
            await cache.SetAsync(query.CacheKey, result.Value, query.Expiration, cancellationToken);
        }

        return result;
    }
}