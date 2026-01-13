namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class CategoryEndpoints : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("categories").WithTags("Categories");

        // GET /categories
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCategoriesQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        });

        // POST /categories (Admin only - ideally)
        group.MapPost("/", async (CreateCategoryRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateCategoryCommand(request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .RequireAuthorization();
    }

    public record CreateCategoryRequest(string Name);
}