namespace BlogApp.Shared.Infrastructure.Messaging;

internal sealed class InMemorySender(IServiceProvider serviceProvider) : ISender
{
    // 1. Command with NO return value (Void/Unit)
    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        return await ExecutePipeline<Result>(command, cancellationToken);
    }

    // 2. Command WITH return value
    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        return await ExecutePipeline<Result<TResponse>>(command, cancellationToken);
    }

    // 3. Query (Fixed: Renamed from 'Query' to 'Send' to match Interface)
    public async Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return await ExecutePipeline<Result<TResponse>>(query, cancellationToken);
    }

    private async Task<TResponse> ExecutePipeline<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Logic to find the correct generic handler interface
        // We look for ICommandHandler<Req, Res> or IQueryHandler<Req, Res>
        var innerType = responseType.IsGenericType
            ? responseType.GetGenericArguments()[0]
            : typeof(object);

        Type handlerType;

        if (typeof(TResponse) == typeof(Result))
        {
            // Non-generic Command
            handlerType = typeof(ICommandHandler<>).MakeGenericType(requestType);
        }
        else
        {
            // Generic Command or Query
            var cmdHandlerType = typeof(ICommandHandler<,>).MakeGenericType(requestType, innerType);

            // Check if it's a Command Handler
            if (serviceProvider.GetService(cmdHandlerType) != null)
            {
                handlerType = cmdHandlerType;
            }
            else
            {
                // Must be a Query Handler
                handlerType = typeof(IQueryHandler<,>).MakeGenericType(requestType, innerType);
            }
        }

        var handler = serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // Setup Behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = (IEnumerable<object>)serviceProvider.GetServices(behaviorType);

        // Create the Pipeline Delegate
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var method = handlerType.GetMethod("Handle")
                         ?? throw new InvalidOperationException($"Handle method not found on {handlerType.Name}");

            return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        };

        // Wrap Behaviors
        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = () =>
            {
                var method = behaviorType.GetMethod("Handle")
                             ?? throw new InvalidOperationException($"Handle method not found on behavior");

                return (Task<TResponse>)method.Invoke(behavior, [request, next, cancellationToken])!;
            };
        }

        return await pipeline();
    }
}