namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

public sealed record CreateArticleCommand(
    string Title,
    string Content,
    string? Summary,
    Guid? CategoryId,
    List<string> Tags) : ICommand<CreateArticleResponse>;

// Define the response DTO
public sealed record CreateArticleResponse(Guid Id, string Slug);