namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class GetMyArticlesEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Notice the URL: "articles/mine"
        app.MapGet("articles/mine", async (
            [AsParameters] GetMyArticlesRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetMyArticlesQuery(
                request.Search,
                request.Page ?? 1,
                request.PageSize ?? 10
            );

            var result = await sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
        })
        .WithTags("Articles")
        .RequireAuthorization() // Crucial!
        .WithSummary("Get current user's articles (Dashboard)");
    }

    public record GetMyArticlesRequest(string? Search, int? Page, int? PageSize);
}