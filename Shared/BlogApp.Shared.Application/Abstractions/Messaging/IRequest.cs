namespace BlogApp.Shared.Application.Abstractions.Messaging;

// Base marker for all internal requests
public interface IRequest<out TResponse> { }