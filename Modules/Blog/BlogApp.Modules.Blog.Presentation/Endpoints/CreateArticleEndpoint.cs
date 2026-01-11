namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class CreateArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("articles", async (
            [FromBody] CreateArticleRequest request,
            ISender sender,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            // 1. Extract User ID from JWT
            // The "sub" claim contains the Guid. 
            // If extracting fails, we return Unauthorized (or let middleware handle it).
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // 2. Create Command
            var command = new CreateArticleCommand(
                userId,
                request.Title,
                request.Content,
                request.Summary,
                request.Tags ?? []);

            // 3. Send
            Result<Guid> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(new { ArticleId = result.Value });
        })
        .WithTags("Articles")
        .RequireAuthorization() // <--- CRITICAL: Only logged-in users
        .WithSummary("Create a new draft article");
    }
}

public record CreateArticleRequest(
    string Title,
    string Content,
    string? Summary,
    List<string>? Tags);