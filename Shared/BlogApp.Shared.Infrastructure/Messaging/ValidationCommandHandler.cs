namespace BlogApp.Shared.Infrastructure.Messaging;

// 1. Non-Generic Decorator
public sealed class ValidationCommandHandler<TCommand>(
    ICommandHandler<TCommand> decorated,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await decorated.Handle(command, cancellationToken);
        }

        var context = new ValidationContext<TCommand>(command);

        // Run all validators in parallel for speed
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage, []))
                .ToArray();

            return Result.Failure(Error.Validation("Validation.Error", "One or more validation errors occurred", errors));
        }

        return await decorated.Handle(command, cancellationToken);
    }
}

// 2. Generic Decorator
public sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> decorated,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await decorated.Handle(command, cancellationToken);
        }

        var context = new ValidationContext<TCommand>(command);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage, []))
                .ToArray();

            return Result.Failure<TResponse>(
                Error.Validation("Validation.Error", "One or more validation errors occurred", errors)
            );
        }

        return await decorated.Handle(command, cancellationToken);
    }
}