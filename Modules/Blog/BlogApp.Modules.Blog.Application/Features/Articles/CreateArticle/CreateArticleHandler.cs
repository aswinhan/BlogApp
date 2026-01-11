namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

internal sealed class CreateArticleHandler(IBlogDbContext context)
    : ICommandHandler<CreateArticleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the Article
        var article = Article.Create(
            request.AuthorId,
            request.Title,
            request.Content,
            request.Summary);

        // 2. Handle Tags (Dedup logic)
        if (request.Tags.Count != 0)
        {
            // Normalize tags to lower case
            var tagNames = request.Tags.Select(t => t.Trim().ToLowerInvariant()).Distinct().ToList();

            // Find existing tags in DB
            var existingTags = await context.Tags
                .Where(t => tagNames.Contains(t.Name))
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

        return article.Id;
    }
}