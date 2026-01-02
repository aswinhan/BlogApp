namespace BlogApp.Shared.Presentation.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Can't convert success result to problem details");
        }

        return Results.Problem(
            statusCode: GetStatusCode(result.Error.Type),
            title: GetTitle(result.Error.Type),
            detail: result.Error.Description,
            extensions: new Dictionary<string, object?>
            {
                { "errors", new[] { result.Error } }
            }
        );
    }

    private static int GetStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(ErrorType type) =>
        type switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Server Error"
        };
}