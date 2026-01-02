namespace BlogApp.Modules.Identity.Application.Features.Users.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;

public record LoginResponse(string AccessToken, string RefreshToken);