namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

// Implement ICacheableQuery
public record GetArticleQuery(Guid ArticleId)
    : IQuery<ArticleResponse>, ICacheableQuery
{
    // Define the Key
    public string CacheKey => $"articles:{ArticleId}";

    // Define the Time (e.g., 10 minutes)
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}