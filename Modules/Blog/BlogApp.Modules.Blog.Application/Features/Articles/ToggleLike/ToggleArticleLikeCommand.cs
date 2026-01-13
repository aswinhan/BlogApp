namespace BlogApp.Modules.Blog.Application.Features.Articles.ToggleLike;

public sealed record ToggleArticleLikeCommand(Guid ArticleId) : ICommand;
