namespace BlogApp.Modules.Blog.Application.Features.Articles.PublishArticle;

public record PublishArticleCommand(Guid ArticleId, Guid UserId) : ICommand;