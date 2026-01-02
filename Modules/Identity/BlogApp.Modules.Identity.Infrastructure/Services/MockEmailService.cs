using BlogApp.Modules.Identity.Application.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace BlogApp.Modules.Identity.Infrastructure.Services;

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        // In the real world, you would use SmtpClient or SendGrid here.
        // For now, we log it to the console so you can click the link.
        logger.LogInformation("==================================================================");
        logger.LogInformation($"[EMAIL SENT] To: {email}");
        logger.LogInformation($"[SUBJECT] Password Reset Request");
        logger.LogInformation($"[BODY] Click here to reset: {resetLink}");
        logger.LogInformation("==================================================================");

        return Task.CompletedTask;
    }
}