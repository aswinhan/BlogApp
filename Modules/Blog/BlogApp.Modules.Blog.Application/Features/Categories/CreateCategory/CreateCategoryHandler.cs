namespace BlogApp.Modules.Blog.Application.Features.Categories.CreateCategory;

internal sealed class CreateCategoryHandler(IBlogDbContext context)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        string slug = SlugGenerator.Generate(request.Name);

        // Check for duplicate slug (optional but good)
        // ...

        var category = Category.Create(request.Name, slug);
        context.Categories.Add(category);
        await context.SaveChangesAsync(ct);

        return Result.Success(category.Id);
    }
}