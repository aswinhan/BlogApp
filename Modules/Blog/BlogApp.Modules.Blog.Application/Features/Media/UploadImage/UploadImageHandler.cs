namespace BlogApp.Modules.Blog.Application.Features.Media.UploadImage;

internal sealed class UploadImageHandler(IFileStorageService fileService)
    : ICommandHandler<UploadImageCommand, string>
{
    public async Task<Result<string>> Handle(UploadImageCommand request, CancellationToken ct)
    {
        try
        {
            // We use "article-covers" as the container name.
            // Your LocalFileStorageService will create: wwwroot/uploads/article-covers/
            string url = await fileService.SaveFileAsync(
                request.File,
                "article-covers",
                null,
                ct);

            return Result.Success(url);
        }
        catch (Exception ex)
        {
            // Log ex here if needed
            return Result.Failure<string>(Error.Failure("Upload.Failed", ex.Message));
        }
    }
}