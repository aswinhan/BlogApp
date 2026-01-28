namespace BlogApp.Shared.Infrastructure.Behaviors;

// Removed 'where TResponse : Result' to avoid compiler constraint issues.
// We check for Result at runtime instead.
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();

        logger.LogInformation("[START] {RequestName} [{RequestGuid}]", requestName, requestGuid);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            // Safe runtime check for Result type
            if (response is Result result && result.IsFailure)
            {
                logger.LogWarning(
                    "[FAILURE] {RequestName} [{RequestGuid}] took {Duration}ms. Error: {ErrorType} - {Error}",
                    requestName,
                    requestGuid,
                    stopwatch.ElapsedMilliseconds,
                    result.Error.Type,
                    result.Error.Description);
            }
            else
            {
                logger.LogInformation(
                    "[SUCCESS] {RequestName} [{RequestGuid}] took {Duration}ms",
                    requestName,
                    requestGuid,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "[ERROR] {RequestName} [{RequestGuid}] failed after {Duration}ms", requestName, requestGuid, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}