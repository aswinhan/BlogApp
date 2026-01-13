namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class UpdateArticleEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("articles/{id:guid}", async (
            Guid id,
            [FromBody] UpdateRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateArticleCommand(
                id,
                request.Title,
                request.Content,
                request.Summary,
                request.CategoryId);

            Result result = await sender.Send(command, cancellationToken);

            return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
        })
        .WithTags("Articles")
        .RequireAuthorization() // Only logged-in users can update
        .WithSummary("Update an existing article");
    }

    // Helper Record to match JSON body
    public record UpdateRequest(string Title, string Content, string? Summary, Guid? CategoryId);
}