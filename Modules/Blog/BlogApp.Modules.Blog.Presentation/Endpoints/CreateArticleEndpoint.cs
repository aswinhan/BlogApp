namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class CreateArticleEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("articles", async (
            CreateArticleRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {

            var command = new CreateArticleCommand(
                request.Title,
                request.Content,
                request.Summary,
                request.CategoryId,
                request.Tags ?? []);

            Result<CreateArticleResponse> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Articles")
        .RequireAuthorization()
        .WithSummary("Create a new article");
    }

    // Helper Record to match the incoming JSON body
    public record CreateArticleRequest(
        string Title,
        string Content,
        string? Summary,
        Guid? CategoryId,
        List<string>? Tags);
}