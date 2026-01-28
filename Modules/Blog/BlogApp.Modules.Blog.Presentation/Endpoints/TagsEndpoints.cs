namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class TagsEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("tags").WithTags("Tags");

        // GET /tags
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var query = new GetTagsQuery();
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .WithSummary("Get all tags with usage count");

        // DELETE /tags/{id}
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var command = new DeleteTagCommand(id);
            var result = await sender.Send(command, ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
        })
        .RequireAuthorization() // Security!
        .WithSummary("Delete a tag");
    }
}