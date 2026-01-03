namespace BlogApp.Shared.Infrastructure.Messaging;

public sealed class InMemorySender(IServiceProvider serviceProvider) : ISender
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> MethodCache = new();

    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        var method = MethodCache.GetOrAdd(commandType, type =>
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(type);
            return handlerType.GetMethod(nameof(ICommandHandler<ICommand>.Handle))
                   ?? throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        });

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var handler = serviceProvider.GetRequiredService(handlerType);

        return await (Task<Result>)method.Invoke(handler, [command, cancellationToken])!;
    }

    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        var method = MethodCache.GetOrAdd(commandType, type =>
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(type, typeof(TResponse));
            return handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.Handle))
                   ?? throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        });

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        return await (Task<Result<TResponse>>)method.Invoke(handler, [command, cancellationToken])!;
    }

    public async Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();

        var method = MethodCache.GetOrAdd(queryType, type =>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TResponse));
            return handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.Handle))
                   ?? throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");
        });

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        return await (Task<Result<TResponse>>)method.Invoke(handler, [query, cancellationToken])!;
    }
}