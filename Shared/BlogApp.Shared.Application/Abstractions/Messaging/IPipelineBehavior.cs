namespace BlogApp.Shared.Application.Abstractions.Messaging;

// 1. Define a delegate representing the "next" step in the pipeline
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

// 2. The interface for any behavior (Logging, Caching, Validation, etc.)
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}