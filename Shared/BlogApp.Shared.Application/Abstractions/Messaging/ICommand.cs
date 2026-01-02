namespace BlogApp.Shared.Application.Abstractions.Messaging;

// Marker interface for commands (write operations)
public interface ICommand : IRequest<Result> { }

// Command returning a value (e.g., Created ID)
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

// Base Request interface (Internal use)
public interface IRequest<out TResponse> { }