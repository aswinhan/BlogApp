namespace BlogApp.Shared.Infrastructure.Messaging;

// 1. Non-Generic Decorator (Void Command)
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

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                .ToList();

            // FIX: Only ONE return statement here.
            // Returning the first error found.
            return Result.Failure(errors[0]);
        }

        return await decorated.Handle(command, cancellationToken);
    }
}

// 2. Generic Decorator (Result<T>)
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
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                .ToList();

            // FIX: Using the generic helper method we added to Result.cs
            return Result.Failure<TResponse>(errors[0]);
        }

        return await decorated.Handle(command, cancellationToken);
    }
}