namespace BlogApp.Shared.Infrastructure.Messaging;

public sealed class InMemorySender(IServiceProvider serviceProvider) : ISender
{
    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler not found for command {command.GetType().Name}");
        return await ((dynamic)handler).Handle((dynamic)command, cancellationToken);
    }

    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler not found for command {command.GetType().Name}");
        return await ((dynamic)handler).Handle((dynamic)command, cancellationToken);
    }

    public async Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler not found for query {query.GetType().Name}");
        return await ((dynamic)handler).Handle((dynamic)query, cancellationToken);
    }
}