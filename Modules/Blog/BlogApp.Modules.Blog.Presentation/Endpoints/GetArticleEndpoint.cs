namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class GetArticleEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("articles/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetArticleQuery(id);

            Result<ArticleResponse> result = await sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Articles")
        .WithSummary("Get article by ID")
        .AllowAnonymous(); // Public access!
    }
}