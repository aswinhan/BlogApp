namespace BlogApp.Modules.Blog.Application.Features.Media.UploadImage;

// We pass the IFormFile directly
public sealed record UploadImageCommand(IFormFile File) : ICommand<string>;