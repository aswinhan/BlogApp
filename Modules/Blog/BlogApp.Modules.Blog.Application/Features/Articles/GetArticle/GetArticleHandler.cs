using BlogApp.Shared.Application.Abstractions.PublicApi;

namespace BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;

internal sealed class GetArticleHandler(
    IBlogDbContext context,
    ICurrentUser currentUser,
    IUserApi userApi)
    : IQueryHandler<GetArticleQuery, ArticleResponse>
{
    public async Task<Result<ArticleResponse>> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        // 1. FETCH ARTICLE
        // Removed .AsSplitQuery() to fix the error.
        var article = await context.Articles
            .AsTracking()
            .Include(a => a.Tags)
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article is null)
        {
            return Result.Failure<ArticleResponse>(Error.NotFound("Article.NotFound", "The article was not found."));
        }

        // 2. VIEW COUNT LOGIC
        Guid? requestingUserId = null;
        try
        {
            requestingUserId = currentUser.UserId;
        }
        catch { /* Anonymous user */ }

        if (requestingUserId != article.AuthorId)
        {
            // Ensure you added this method to Article.cs!
            article.IncrementViewCount();
            await context.SaveChangesAsync(cancellationToken);
        }

        // 3. LIKE SYSTEM LOGIC
        var likeCountTask = context.ArticleLikes
            .CountAsync(x => x.ArticleId == article.Id, cancellationToken);

        var isLikedByMeTask = Task.FromResult(false);
        if (requestingUserId.HasValue)
        {
            isLikedByMeTask = context.ArticleLikes
                .AnyAsync(x => x.ArticleId == article.Id && x.UserId == requestingUserId.Value, cancellationToken);
        }

        await Task.WhenAll(likeCountTask, isLikedByMeTask);

        // FETCH AUTHOR INFO
        var author = await userApi.GetUserAsync(article.AuthorId, cancellationToken);
        string authorName = author is not null
            ? $"{author.FirstName} {author.LastName}"
            : "Unknown Author";

        // 4. MAP & RETURN
        var response = new ArticleResponse(
            article.Id,
            article.AuthorId,
            article.Title,
            article.Content,
            article.Summary,
            article.Slug,
            article.Status.ToString(),
            article.CoverImageUrl,
            article.ViewCount,
            likeCountTask.Result,
            isLikedByMeTask.Result,
            article.Category is null
                ? null
                : new CategoryResponse(article.Category.Id, article.Category.Name, article.Category.Slug),
            article.Tags.Select(t => t.Name).ToList(),
            article.CreatedOnUtc,
            article.PublishedOnUtc,
            authorName
        );

        return Result.Success(response);
    }
}