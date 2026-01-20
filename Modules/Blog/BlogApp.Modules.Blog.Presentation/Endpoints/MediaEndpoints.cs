namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class MediaEndpoints : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("media/upload", async (
            IFormFile file, // Minimal API binds this automatically
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UploadImageCommand(file);
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            // Return the URL
            return Results.Ok(new { Url = result.Value });
        })
        .WithTags("Media")
        .DisableAntiforgery() // Sometimes needed for file uploads in basic setups
        .RequireAuthorization();
    }
}