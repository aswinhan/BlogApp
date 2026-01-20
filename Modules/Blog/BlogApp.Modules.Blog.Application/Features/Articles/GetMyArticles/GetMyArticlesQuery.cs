namespace BlogApp.Modules.Blog.Application.Features.Articles.GetMyArticles;

public sealed record GetMyArticlesQuery(
    string? SearchTerm,
    int Page,
    int PageSize) : IQuery<PagedList<DashboardArticleResponse>>;

public sealed record DashboardArticleResponse(
    Guid Id,
    string Title,
    string Slug,
    string Status, // <--- Critical for Dashboard
    string? CoverImageUrl,
    string CategoryName,
    DateTime CreatedOnUtc,
    DateTime? PublishedOnUtc,
    long ViewCount,
    int LikeCount);