namespace BlogApp.Shared.Infrastructure.Messaging;

// 1. Non-Generic Decorator (Returns Result)
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

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage, []))
                .ToArray();

            // FIXED: Passing the 'errors' array as the 3rd argument
            return Result.Failure(Error.Validation("Validation.Error", "Validation Failed", errors));
        }

        return await decorated.Handle(command, cancellationToken);
    }
}

// 2. Generic Decorator (Returns Result<TResponse>)
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

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage, []))
                .ToArray();

            // FIXED: Passing 'errors' array AND Explicit Generic Type <TResponse>
            return Result.Failure<TResponse>(
                Error.Validation("Validation.Error", "Validation Failed", errors)
            );
        }

        return await decorated.Handle(command, cancellationToken);
    }
}