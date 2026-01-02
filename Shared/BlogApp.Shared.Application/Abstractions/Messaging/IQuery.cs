namespace BlogApp.Shared.Application.Abstractions.Messaging;

// Query returning a value (Read operations)
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }