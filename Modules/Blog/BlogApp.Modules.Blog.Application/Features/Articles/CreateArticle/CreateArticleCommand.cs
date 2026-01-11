namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

// We return the Guid of the created article
public record CreateArticleCommand(
    Guid AuthorId, // In a real app, we extract this from the JWT (Claims)
    string Title,
    string Content,
    string? Summary,
    List<string> Tags) : ICommand<Guid>;