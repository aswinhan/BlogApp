using BlogApp.Modules.Blog.Application.Metrics;
namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

internal sealed class CreateArticleHandler(IBlogDbContext context, ICurrentUser currentUser, BlogMetrics metrics) : ICommandHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IBlogDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<CreateArticleResponse>> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        // 1. Generate Slug
        var slug = SlugGenerator.Generate(command.Title);

        // 2. Uniqueness Check
        var slugExists = await _context.Articles
            .AnyAsync(a => a.Slug == slug, cancellationToken);

        if (slugExists)
        {
            // FIX: Use static factory method 'Error.Conflict' instead of 'new Error'
            // Implicit conversion handles Result<CreateArticleResponse>
            return Error.Conflict(
                "Article.DuplicateSlug",
                "An article with this title already exists.");
        }

        // 3. Create the Article
        var article = Article.Create(
            command.Title,
            command.Content,
            command.Summary,
            _currentUser.UserId,
            slug,
            command.CategoryId
        );

        // 4. Handle Tags
        if (command.Tags is { Count: > 0 })
        {
            var distinctTagNames = command.Tags.Distinct().ToList();

            var existingTags = await _context.Tags
                .Where(t => distinctTagNames.Contains(t.Name))
                .ToListAsync(cancellationToken);

            foreach (var tagName in distinctTagNames)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name == tagName);

                if (tag is null)
                {
                    tag = Tag.Create(tagName);
                    _context.Tags.Add(tag);
                }

                article.AddTag(tag);
            }
        }

        // 5. Persist
        _context.Articles.Add(article);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Record Metric
        metrics.ArticleCreated();
        if (command.PublishedOnUtc.HasValue && command.PublishedOnUtc <= DateTime.UtcNow)
        {
            metrics.ArticlePublished();
        }

        // 7. Return Success
        return new CreateArticleResponse(article.Id, article.Slug);
    }
}