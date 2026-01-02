namespace BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;