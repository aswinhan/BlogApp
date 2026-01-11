namespace BlogApp.Shared.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorageService> _logger;

    // These fields are calculated once in the constructor for performance
    private readonly string _storageRootPath;
    private readonly string _serveUrlBase;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IOptions<FileStorageSettings> options,
        ILogger<LocalFileStorageService> logger)
    {
        _environment = environment;
        _settings = options.Value;
        _logger = logger;

        // 1. Calculate the physical path on the server (e.g., C:\app\wwwroot\uploads)
        _storageRootPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, _settings.LocalBasePath);

        // 2. Calculate the URL prefix (e.g., /uploads)
        // Ensure it starts with / and doesn't end with /
        var prefix = _settings.LocalServePrefix.Trim('/');
        _serveUrlBase = $"/{prefix}";

        // 3. Ensure directory exists immediately on startup
        if (!Directory.Exists(_storageRootPath))
        {
            Directory.CreateDirectory(_storageRootPath);
            _logger.LogInformation("Created local storage directory: {Path}", _storageRootPath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file, string containerName, string? blobName = null, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.", nameof(file));
        }

        // Sanitize container name to prevent path traversal attacks (e.g., "../")
        var safeContainerName = Path.GetFileName(containerName);
        var containerPath = Path.Combine(_storageRootPath, safeContainerName);

        if (!Directory.Exists(containerPath))
        {
            Directory.CreateDirectory(containerPath);
        }

        var extension = Path.GetExtension(file.FileName);
        // Use provided name or generate a new GUID
        var fileName = string.IsNullOrWhiteSpace(blobName)
            ? $"{Guid.NewGuid()}{extension}"
            : Path.GetFileName(blobName); // Sanitize blob name too

        var filePath = Path.Combine(containerPath, fileName);

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            // Construct URL: /uploads/container/filename.ext
            // We use Path.AltDirectorySeparatorChar ('/') to ensure URL compatibility on Windows
            string fileUrl = $"{_serveUrlBase}/{safeContainerName}/{fileName}";
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file {FileName}", file.FileName);
            throw;
        }
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        try
        {
            // 1. Security Check: Ensure the URL belongs to our storage
            if (!fileUrl.StartsWith(_serveUrlBase, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Cannot delete file: URL {FileUrl} does not match serve prefix {ServePrefix}", fileUrl, _serveUrlBase);
                return Task.CompletedTask;
            }

            // 2. Parse Path: Extract "/container/filename.ext"
            var relativeUrl = fileUrl.Substring(_serveUrlBase.Length).TrimStart('/');

            // 3. Convert to Physical Path: C:\app\wwwroot\uploads\container\filename.ext
            // We replace URL slashes with the OS specific separator
            var relativeSystemPath = relativeUrl.Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(_storageRootPath, relativeSystemPath);

            if (File.Exists(filePath))
            {
                _logger.LogInformation("Deleting file: {FilePath}", filePath);
                File.Delete(filePath);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file corresponding to URL: {FileUrl}", fileUrl);
        }

        return Task.CompletedTask;
    }
}