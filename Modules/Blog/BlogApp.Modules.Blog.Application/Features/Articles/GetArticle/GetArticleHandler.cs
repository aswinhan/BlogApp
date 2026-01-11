namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

internal sealed class GetArticleHandler(IBlogDbContext context)
    : IQueryHandler<GetArticleQuery, ArticleResponse>
{
    public async Task<Result<ArticleResponse>> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var article = await context.Articles
            .AsNoTracking() // Performance: Read-only
            .Where(a => a.Id == request.ArticleId)
            .Select(a => new ArticleResponse(
                a.Id,
                a.AuthorId,
                a.Title,
                a.Content,
                a.Summary,
                a.Status.ToString(), // Convert Enum to String
                a.Tags.Select(t => t.Name).ToList(),
                a.CreatedOnUtc,
                a.PublishedOnUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (article is null)
        {
            return Result.Failure<ArticleResponse>(Error.NotFound("Article.NotFound", "The article with the specified ID was not found."));
        }

        return article;
    }
}