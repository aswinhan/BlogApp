namespace BlogApp.Shared.Infrastructure.Database;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        // Optimize: If count is 0, don't query DB for items
        if (count == 0)
        {
            return PagedList<T>.Create([], page, pageSize, 0);
        }

        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedList<T>.Create(items, page, pageSize, count);
    }
}