namespace BlogApp.Modules.Blog.Application.Features.Categories.GetCategories;

public sealed record GetCategoriesQuery : IQuery<List<CategoryResponse>>;
public sealed record CategoryResponse(Guid Id, string Name, string Slug);