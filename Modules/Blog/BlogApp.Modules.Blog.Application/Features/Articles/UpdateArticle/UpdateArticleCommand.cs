namespace BlogApp.Modules.Blog.Application.Features.Articles.UpdateArticle;

public sealed record UpdateArticleCommand(
    Guid ArticleId,
    string Title,
    string Content,
    string? Summary,
    Guid? CategoryId) : ICommand;