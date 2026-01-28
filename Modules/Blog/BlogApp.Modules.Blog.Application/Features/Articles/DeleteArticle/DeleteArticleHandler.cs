namespace BlogApp.Modules.Blog.Application.Features.Articles.DeleteArticle;

internal sealed class DeleteArticleHandler(
    IBlogDbContext context,
    ICurrentUser currentUser)
    : ICommandHandler<DeleteArticleCommand>
{
    public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Article
        // Note: Because of Global Query Filter, this automatically returns null 
        // if the article is already soft-deleted.
        var article = await context.Articles
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null)
        {
            return Result.Failure(Error.NotFound("Article.NotFound", "The article was not found."));
        }

        // 2. Security Check (The Missing Piece)
        // "Is the logged-in user the owner of this article?"
        if (article.AuthorId != currentUser.UserId)
        {
            return Result.Failure(Error.Forbidden("Security.Forbidden", "You are not allowed to delete this article."));
        }

        // 3. Perform Soft Delete
        article.SoftDelete();

        // 4. Save
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}