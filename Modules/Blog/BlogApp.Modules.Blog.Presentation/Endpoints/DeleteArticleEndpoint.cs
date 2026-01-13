namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class DeleteArticleEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("articles/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteArticleCommand(id);

            Result result = await sender.Send(command, cancellationToken);

            return result.IsSuccess ? Results.NoContent() : result.ToProblemDetails();
        })
        .WithTags("Articles")
        .RequireAuthorization()
        .WithSummary("Soft delete an article");
    }
}