namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticles;

internal sealed class GetArticlesHandler(IBlogDbContext context)
    : IQueryHandler<GetArticlesQuery, PagedList<ArticleSummaryResponse>>
{
    public async Task<Result<PagedList<ArticleSummaryResponse>>> Handle(GetArticlesQuery request, CancellationToken ct)
    {
        // =========================================================================
        // STEP 1: Build the Filtered Query (But don't Select/Project yet)
        // =========================================================================
        var query = context.Articles
            .AsNoTracking()
            .Where(a => a.Status == ArticleStatus.Published && !a.IsDeleted);

        // Apply Filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(a =>
                a.Title.Contains(request.SearchTerm) ||
                (a.Summary != null && a.Summary.Contains(request.SearchTerm)));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == request.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Tag))
        {
            query = query.Where(a => a.Tags.Any(t => t.Name == request.Tag));
        }

        // =========================================================================
        // STEP 2: Pagination Logic
        // =========================================================================
        int totalCount = await query.CountAsync(ct);

        // Fetch the "Raw" Articles first (including Navigations needed for display)
        var articles = await query
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .OrderByDescending(a => a.PublishedOnUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // =========================================================================
        // STEP 3: Fetch Like Counts (Batch Query)
        // =========================================================================
        // Get IDs of the articles we just fetched
        var articleIds = articles.Select(a => a.Id).ToList();

        // Query the Likes table only for these specific IDs
        // "Group by ArticleId and Count them"
        var likeCounts = await context.ArticleLikes // Note: Using direct DbSet access (Make sure IBlogDbContext exposes this!)
            .Where(l => articleIds.Contains(l.ArticleId))
            .GroupBy(l => l.ArticleId)
            .Select(g => new { ArticleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.ArticleId, v => v.Count, ct);

        // =========================================================================
        // STEP 4: Merge & Map in Memory
        // =========================================================================
        var responseItems = articles.Select(a => new ArticleSummaryResponse(
            a.Id,
            a.Title,
            a.Slug,
            a.Summary,
            a.CoverImageUrl,
            a.Category?.Name ?? "Uncategorized", // Safe null access
            a.Tags.Select(t => t.Name).ToList(),
            a.AuthorId,
            a.PublishedOnUtc ?? a.CreatedOnUtc,
            a.ViewCount,
            // Lookup the count from our dictionary (default to 0 if not found)
            likeCounts.GetValueOrDefault(a.Id, 0)
        )).ToList();

        // =========================================================================
        // STEP 5: Return
        // =========================================================================
        var pagedList = PagedList<ArticleSummaryResponse>.Create(
            responseItems,
            request.Page,
            request.PageSize,
            totalCount);

        return Result.Success(pagedList);
    }
}