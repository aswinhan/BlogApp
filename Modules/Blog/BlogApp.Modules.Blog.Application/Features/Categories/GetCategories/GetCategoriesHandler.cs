namespace BlogApp.Modules.Blog.Application.Features.Categories.GetCategories;

internal sealed class GetCategoriesHandler(IBlogDbContext context)
    : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Slug))
            .ToListAsync(ct);

        return Result.Success(categories);
    }
}