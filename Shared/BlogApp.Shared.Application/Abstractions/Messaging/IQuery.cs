namespace BlogApp.Shared.Application.Abstractions.Messaging;

// Queries always return data, never void
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }