namespace BlogApp.Modules.Blog.Application.Features.Comments.AddComment;

internal sealed class AddCommentHandler(IBlogDbContext context)
    : ICommandHandler<AddCommentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Parent (NO TRACKING needed)
        // We only need to check if it exists. We are not changing the Article itself.
        var article = await context.Articles
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null) return Result.Failure<Guid>(Error.NotFound("Article.NotFound", "Article not found"));

        // 2. Create the New Entity (Domain Logic)
        // We use the Domain Method to ensure rules are followed, but we grab the result.
        article.AddComment(request.UserId, request.Content);
        var newComment = article.Comments.Last();

        // 3. EXPLICIT ADD (Pattern 1)
        // Consistent with CreateArticleHandler
        context.Comments.Add(newComment);

        // 4. Save
        await context.SaveChangesAsync(cancellationToken);

        return newComment.Id;
    }
}