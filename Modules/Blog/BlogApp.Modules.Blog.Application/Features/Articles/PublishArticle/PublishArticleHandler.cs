namespace BlogApp.Modules.Blog.Application.Features.Articles.PublishArticle;

internal sealed class PublishArticleHandler(IBlogDbContext context)
    : ICommandHandler<PublishArticleCommand>
{
    public async Task<Result> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch with TRACKING (Pattern 2)
        // We explicitly ask EF Core to track changes for this transaction.
        var article = await context.Articles
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null)
        {
            return Result.Failure(Error.NotFound("Article.NotFound", "Article not found"));
        }

        // 2. Security Check (Authorization)
        // Only the original author can publish it.
        if (article.AuthorId != request.UserId)
        {
            return Result.Failure(Error.Failure("Article.Unauthorized", "You are not the author of this article"));
        }

        // 3. Execute Domain Logic
        // We don't set 'Status' directly. We call the method.
        // This ensures PublishedOnUtc is set correctly.
        article.Publish();

        // 4. Save
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}