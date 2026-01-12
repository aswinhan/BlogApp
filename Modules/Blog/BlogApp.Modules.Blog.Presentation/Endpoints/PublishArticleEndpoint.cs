using BlogApp.Modules.Blog.Domain.Constants;

namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class PublishArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("articles/{id:guid}/publish", async (
            Guid id,
            ISender sender,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            // 1. Get Current User
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            // 2. Send Command
            var command = new PublishArticleCommand(id, userId);
            Result result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.NoContent(); // 204 No Content (Standard for Updates)
        })
        .WithTags("Articles")
        .RequireAuthorization(BlogPolicyConsts.PublishArticle)
        .WithSummary("Publish a draft article");
    }
}