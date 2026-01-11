namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

public record ArticleResponse(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Content,
    string? Summary,
    string Status,
    List<string> Tags,
    DateTime CreatedOnUtc,
    DateTime? PublishedOnUtc);