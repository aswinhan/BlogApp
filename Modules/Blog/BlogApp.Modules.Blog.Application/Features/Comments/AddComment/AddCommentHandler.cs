namespace BlogApp.Modules.Blog.Application.Features.Comments.AddComment;

internal sealed class AddCommentHandler(
    IBlogDbContext context,
    ICurrentUser currentUser)
    : ICommandHandler<AddCommentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken ct)
    {
        // 1. Verify Article Exists
        var article = await context.Articles
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, ct);

        if (article is null)
        {
            return Result.Failure<Guid>(Error.NotFound("Article.NotFound", "Article not found"));
        }

        // 2. Create Comment
        // We use the Domain Method on Article to encapsulate logic (if you added it to Article.cs)
        // OR we can add it directly to the DbSet. 
        // Using the Article method is cleaner if you have it: article.AddComment(...)

        // Let's use the direct approach for flexibility here:
        var comment = Comment.Create(request.ArticleId, currentUser.UserId, request.Content);

        context.Comments.Add(comment);
        await context.SaveChangesAsync(ct);

        return Result.Success(comment.Id);
    }
}