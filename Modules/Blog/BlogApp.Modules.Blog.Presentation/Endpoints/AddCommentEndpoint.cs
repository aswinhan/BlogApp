namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class AddCommentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("articles/{id:guid}/comments", async (
            Guid id,
            [FromBody] AddCommentRequest request,
            ISender sender,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            // 1. Get User
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // 2. Send Command
            var command = new AddCommentCommand(id, userId, request.Content);
            Result<Guid> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(new { CommentId = result.Value });
        })
        .WithTags("Comments")
        .RequireAuthorization()
        .WithSummary("Add a comment to an article");
    }
}

public record AddCommentRequest(string Content);