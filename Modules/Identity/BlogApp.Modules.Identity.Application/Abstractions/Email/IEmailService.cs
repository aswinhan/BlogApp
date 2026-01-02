namespace BlogApp.Modules.Identity.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default);
}