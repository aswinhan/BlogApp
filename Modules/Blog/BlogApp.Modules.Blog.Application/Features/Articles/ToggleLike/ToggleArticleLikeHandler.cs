namespace BlogApp.Modules.Blog.Application.Features.Articles.ToggleLike;

internal sealed class ToggleArticleLikeHandler(
    IBlogDbContext context,
    ICurrentUser currentUser)
    : ICommandHandler<ToggleArticleLikeCommand>
{
    public async Task<Result> Handle(ToggleArticleLikeCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        // 1. Check if the article exists (Optional but safer)
        var articleExists = await context.Articles
            .AnyAsync(a => a.Id == request.ArticleId, ct);

        if (!articleExists)
        {
            return Result.Failure(Error.NotFound("Article.NotFound", "Article not found"));
        }

        // 2. Check if Like exists
        var existingLike = await context.ArticleLikes
            .FirstOrDefaultAsync(x => x.ArticleId == request.ArticleId && x.UserId == userId, ct);

        if (existingLike is not null)
        {
            // UNLIKE: Remove it
            context.ArticleLikes.Remove(existingLike);
        }
        else
        {
            // LIKE: Add it
            context.ArticleLikes.Add(new ArticleLike
            {
                ArticleId = request.ArticleId,
                UserId = userId
            });
        }

        await context.SaveChangesAsync(ct);
        return Result.Success();
    }
}