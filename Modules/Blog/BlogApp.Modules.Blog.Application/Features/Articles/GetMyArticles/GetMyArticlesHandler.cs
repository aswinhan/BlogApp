namespace BlogApp.Modules.Blog.Application.Features.Articles.GetMyArticles;

internal sealed class GetMyArticlesHandler(
    IBlogDbContext context,
    ICurrentUser currentUser)
    : IQueryHandler<GetMyArticlesQuery, PagedList<DashboardArticleResponse>>
{
    public async Task<Result<PagedList<DashboardArticleResponse>>> Handle(GetMyArticlesQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        // 1. Build Query (Scoped to Current User)
        // We do NOT filter by Status here because authors need to see Drafts.
        var query = context.Articles
            .AsNoTracking()
            .Where(a => a.AuthorId == userId && !a.IsDeleted);

        // 2. Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(a =>
                a.Title.Contains(request.SearchTerm) ||
                (a.Summary != null && a.Summary.Contains(request.SearchTerm)));
        }

        // 3. Pagination & Fetch Articles
        int totalCount = await query.CountAsync(ct);

        var articles = await query
            .Include(a => a.Category)
            .OrderByDescending(a => a.CreatedOnUtc) // Newest created first (better for drafts)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // 4. Batch Fetch Likes
        var articleIds = articles.Select(a => a.Id).ToList();

        var likeCounts = await context.ArticleLikes
            .AsNoTracking() // Good practice for read-only
            .Where(l => articleIds.Contains(l.ArticleId))
            .GroupBy(l => l.ArticleId)
            .Select(g => new { ArticleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.ArticleId, v => v.Count, ct);

        // 5. Map to DTO
        var responseItems = articles.Select(a => new DashboardArticleResponse(
            a.Id,
            a.Title,
            a.Slug,
            a.Status.ToString(), // "Draft", "Published", etc.
            a.CoverImageUrl,
            a.Category?.Name ?? "Uncategorized",
            a.CreatedOnUtc,
            a.PublishedOnUtc,
            a.ViewCount,
            likeCounts.GetValueOrDefault(a.Id, 0)
        )).ToList();

        return Result.Success(PagedList<DashboardArticleResponse>.Create(
            responseItems,
            request.Page,
            request.PageSize,
            totalCount));
    }
}