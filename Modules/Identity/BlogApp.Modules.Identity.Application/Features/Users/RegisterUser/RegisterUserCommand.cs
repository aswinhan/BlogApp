namespace BlogApp.Modules.Identity.Application.Features.Users.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : ICommand<Guid>;