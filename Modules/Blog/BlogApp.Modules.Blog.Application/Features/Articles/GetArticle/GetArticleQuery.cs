namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

public record GetArticleQuery(Guid ArticleId) : IQuery<ArticleResponse>;