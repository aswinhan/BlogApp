namespace BlogApp.Shared.Infrastructure.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = validators
            .Select(validator => validator.Validate(context))
            .Where(validationResult => validationResult.Errors.Count > 0)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new Shared.Domain.Errors.ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();

        if (validationErrors.Count > 0)
        {
            // Explicit namespace to avoid ambiguity
            throw new BlogApp.Shared.Application.Exceptions.ValidationException(validationErrors);
        }

        return await next();
    }
}