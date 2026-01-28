using BlogApp.Modules.Blog.Application.Metrics;

namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class TestMetricsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // This endpoint is just for debugging metrics
        // Explicitly using [FromServices] to prevent "Body Deserialization" error
        app.MapPost("test/metrics/increment", ([FromServices] BlogMetrics metrics) =>
        {
            // Manually increment the counters
            metrics.ArticleCreated();
            metrics.ArticlePublished();

            return Results.Ok(new { Message = "Metrics Incremented Manually. Check Aspire Dashboard." });
        })
        .WithTags("Blog")
        .WithSummary("Test Metrics")
        .AllowAnonymous();
    }
}