namespace BlogApp.Modules.Blog.Application.Features.Tags.GetTags;

internal sealed class GetTagsHandler(IBlogDbContext context)
    : IQueryHandler<GetTagsQuery, List<TagResponse>>
{
    public async Task<Result<List<TagResponse>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        // STEP 1: Fetch Data using an Anonymous Type
        var tagsData = await context.Tags
            .AsNoTracking()
            .Select(t => new
            {
                t.Id,
                t.Name,
                ArticleCount = t.Articles.Count(a => a.Status == ArticleStatus.Published)
            })
            .OrderByDescending(x => x.ArticleCount) 
            .ToListAsync(cancellationToken);

        // STEP 2: Map to Response DTO in Memory
        // This happens in C# (RAM), so it's super fast and safe.
        var response = tagsData
            .Select(t => new TagResponse(t.Id, t.Name, t.ArticleCount))
            .ToList();

        return Result.Success(response);
    }
}