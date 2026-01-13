namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class CommentEndpoints : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("articles/{articleId:guid}/comments").WithTags("Comments");

        // GET Comments
        group.MapGet("/", async (Guid articleId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCommentsQuery(articleId), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .WithSummary("Get comments for an article");

        // POST Comment
        group.MapPost("/", async (Guid articleId, AddCommentRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddCommentCommand(articleId, request.Content);
            var result = await sender.Send(command, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .RequireAuthorization()
        .WithSummary("Add a comment to an article");
    }

    public record AddCommentRequest(string Content);
}