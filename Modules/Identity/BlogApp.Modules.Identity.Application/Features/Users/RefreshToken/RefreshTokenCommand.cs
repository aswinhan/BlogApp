namespace BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponse>;