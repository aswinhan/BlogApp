namespace BlogApp.Shared.Application.Storage;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to the configured storage location.
    /// </summary>
    /// <param name="file">The file to save (from an HTTP request).</param>
    /// <param name="containerName">A logical container/folder name (e.g., "article-covers").</param>
    /// <param name="blobName">Optional: A specific name. If null, a unique GUID is generated.</param>
    /// <returns>The public URL of the saved file.</returns>
    Task<string> SaveFileAsync(IFormFile file, string containerName, string? blobName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}