namespace BlogApp.Web.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Map the Exception to Status Code & Details
        var (statusCode, title, detail, errors) = MapException(exception);

        // 2. Log intelligently
        // 500s are actual bugs -> Log Error
        // 400s are client mistakes -> Log Warning (reduces log noise)
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            logger.LogWarning("Domain exception: {Message}", exception.Message);
        }

        // 3. Create Standard ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Detail = detail
        };

        // 4. Attach Validation Errors (if any)
        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail, object? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            // 1. Domain Guard Clauses (e.g., "Title is required")
            // Maps ArgumentException to 400 Bad Request
            ArgumentException argEx =>
                (StatusCodes.Status400BadRequest, "Bad Request", argEx.Message, null),

            // 2. Application Validation Failures
            // Maps ValidationException to 400 Bad Request + Error List
            ValidationException valEx =>
                (StatusCodes.Status400BadRequest, "Validation Failure", "One or more validation errors occurred.", valEx.Errors),

            // 3. Default Catch-All
            // Maps unknown errors to 500 Server Error
            _ => (StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.", null)
        };
    }
}