namespace BlogApp.Modules.Blog.Application.Features.Articles.DeleteArticle;

public sealed record DeleteArticleCommand(Guid ArticleId) : ICommand;