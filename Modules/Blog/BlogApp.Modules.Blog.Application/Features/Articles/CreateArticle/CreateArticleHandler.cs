
namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

internal sealed class CreateArticleHandler(IBlogDbContext context,ICurrentUser currentUser)
    : ICommandHandler<CreateArticleCommand, CreateArticleResponse>
{
    public async Task<Result<CreateArticleResponse>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate Base Slug
        string baseSlug = SlugGenerator.Generate(request.Title);
        string finalSlug = baseSlug;
        int counter = 1;

        // 2. Check for Uniqueness (Simple Loop)
        // In a massive system, we might use a separate index or unique constraint catch
        while (await context.Articles.AnyAsync(a => a.Slug == finalSlug, cancellationToken))
        {
            finalSlug = $"{baseSlug}-{counter}";
            counter++;
        }

        // 3. Create the Article
        var article = Article.Create(
            currentUser.UserId,
            request.Title,
            request.Content,
            request.Summary,
            finalSlug,
            request.CategoryId);

        // 2. Handle Tags (Dedup logic)
        if (request.Tags.Count != 0)
        {
            // Normalize tags to lower case
            var tagNames = request.Tags.Select(t => t.Trim().ToLowerInvariant()).Distinct().ToList();

            // Find existing tags in DB
            // We must use .AsTracking() here.
            // If we don't, EF Core thinks these are "New" tags and tries to INSERT them again, causing a crash.
            var existingTags = await context.Tags
                .Where(t => tagNames.Contains(t.Name))
                .AsTracking()
                .ToListAsync(cancellationToken);

            foreach (var tagName in tagNames)
            {
                var tag = existingTags.FirstOrDefault(t => t.Name == tagName);

                // Use existing or create new
                if (tag is null)
                {
                    tag = Tag.Create(tagName);
                    context.Tags.Add(tag); // Mark new tag for adding
                }

                article.AddTag(tag);
            }
        }

        // 3. Save
        context.Articles.Add(article);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateArticleResponse(article.Id, finalSlug));
    }
}