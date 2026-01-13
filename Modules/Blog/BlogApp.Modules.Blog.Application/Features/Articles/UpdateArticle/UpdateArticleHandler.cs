
namespace BlogApp.Modules.Blog.Application.Features.Articles.UpdateArticle;

internal sealed class UpdateArticleHandler(IBlogDbContext context, ICurrentUser currentUser)
    : ICommandHandler<UpdateArticleCommand>
{
    public async Task<Result> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Article
        var article = await context.Articles
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null)
        {
            return Result.Failure(Error.NotFound("Article.NotFound", "The article with the specified ID was not found."));
        }

        // 2. Authorization Check
        if (article.AuthorId != currentUser.UserId)
        {
            return Result.Failure(Error.Failure("Security.Forbidden", "You are not allowed to edit this article."));
        }

        // 3. Generate New Slug (Required because Title might have changed)
        string baseSlug = SlugGenerator.Generate(request.Title);
        string finalSlug = baseSlug;
        int counter = 1;

        // Check for Uniqueness, BUT exclude the current article (so it doesn't conflict with itself)
        while (await context.Articles.AnyAsync(a => a.Slug == finalSlug && a.Id != request.ArticleId, cancellationToken))
        {
            finalSlug = $"{baseSlug}-{counter}";
            counter++;
        }

        // 4. Perform the Update
        // Now passing all 4 arguments: Title, Content, Summary, Slug
        article.UpdateDetails(request.Title, request.Content, request.Summary, finalSlug, request.CategoryId);

        // 5. Save Changes
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}