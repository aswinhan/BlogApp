namespace BlogApp.Modules.Blog.Application.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;