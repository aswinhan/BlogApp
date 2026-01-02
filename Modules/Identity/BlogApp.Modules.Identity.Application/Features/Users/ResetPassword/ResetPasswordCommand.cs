namespace BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand;