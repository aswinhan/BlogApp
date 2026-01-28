namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

public sealed record ArticleResponse(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Content,
    string? Summary,
    string Slug, 
    string Status,
    string? CoverImageUrl,
    long ViewCount, 
    int LikeCount,  
    bool IsLikedByMe,  
    CategoryResponse? Category,
    List<string> Tags,
    DateTime CreatedOnUtc,
    DateTime? PublishedOnUtc,
    string AuthorName);