namespace BlogApp.Shared.Application.Abstractions.Messaging;

// 1. Void Command (Returns standard Result)
public interface ICommand : IRequest<Result> { }

// 2. Value Command (Returns Result<T>)
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }