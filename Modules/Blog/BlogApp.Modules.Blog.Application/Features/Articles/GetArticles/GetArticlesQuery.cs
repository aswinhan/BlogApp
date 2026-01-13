namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticles;

// Return PagedList<T> instead of PagedResult<T>
public sealed record GetArticlesQuery(
    string? SearchTerm,
    Guid? CategoryId,
    string? Tag,
    int Page,
    int PageSize) : IQuery<PagedList<ArticleSummaryResponse>>;

public sealed record ArticleSummaryResponse(
    Guid Id,
    string Title,
    string Slug,
    string? Summary,
    string? CoverImageUrl,
    string CategoryName,
    List<string> Tags,
    Guid AuthorId,
    DateTime CreatedOnUtc,
    long ViewCount,
    int LikeCount);