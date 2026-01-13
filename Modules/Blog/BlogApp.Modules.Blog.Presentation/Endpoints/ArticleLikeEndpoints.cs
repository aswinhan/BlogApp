namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class ArticleLikeEndpoints : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Uses PUT because it's idempotent-ish (Toggle)
        app.MapPut("articles/{id:guid}/like", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ToggleArticleLikeCommand(id);
            var result = await sender.Send(command, ct);

            // Return 204 No Content
            return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
        })
        .WithTags("Articles")
        .RequireAuthorization()
        .WithSummary("Toggle like on an article");
    }
}
