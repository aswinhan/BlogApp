namespace BlogApp.Modules.Blog.Presentation.Endpoints;

public class UploadCoverEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("articles/{id:guid}/cover", async (
            Guid id,
            IFormFile file, // Asp.Net Core binds this automatically
            IBlogDbContext context,
            IFileStorageService fileStorage,
            CancellationToken cancellationToken) =>
        {
            // 1. Get Article
            var article = await context.Articles
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (article is null)
            {
                return Results.NotFound(new { error = "Article not found" });
            }

            // 2. Validate File
            if (file.Length == 0 || !file.ContentType.StartsWith("image/"))
            {
                return Results.BadRequest(new { error = "Invalid image file" });
            }

            // 3. Save File using the Shared Service
            // This will save to: wwwroot/uploads/article-covers/{guid}.jpg
            string fileUrl = await fileStorage.SaveFileAsync(
                file,
                "article-covers",
                blobName: $"{id}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                cancellationToken);

            // 4. Update Domain
            article.UpdateCover(fileUrl);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { Url = fileUrl });
        })
        .WithTags("Articles")
        .RequireAuthorization() // Must be logged in
        .DisableAntiforgery() // Often required for file uploads
        .WithSummary("Upload an article cover image");
    }
}