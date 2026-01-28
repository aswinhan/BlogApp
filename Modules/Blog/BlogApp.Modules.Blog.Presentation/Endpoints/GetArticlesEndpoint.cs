namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class GetArticlesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("articles", async (
            [AsParameters] GetArticlesRequest request, // Allows binding query params automatically
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetArticlesQuery(
                request.Search,
                request.CategoryId,
                request.Tag,
                request.Page ?? 1,
                request.PageSize ?? 10
            );

            var result = await sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .WithTags("Articles")
        .WithSummary("Get paged articles with search and filters");
    }

    // Helper for Query Parameters
    public record GetArticlesRequest(
        string? Search,
        Guid? CategoryId,
        string? Tag,
        int? Page,
        int? PageSize);
}