namespace BlogApp.Shared.Application.Abstractions.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    // bool IsAdmin { get; } // Future proofing
}