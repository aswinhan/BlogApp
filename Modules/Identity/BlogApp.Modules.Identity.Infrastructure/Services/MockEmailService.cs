namespace BlogApp.Modules.Identity.Infrastructure.Services;

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default)
    {
        // In the real world, you would use SmtpClient or SendGrid here.
        // For now, we log it to the console so you can click the link.
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("==================================================================");
            logger.LogInformation("[EMAIL SENT] To: {email}", email);
            logger.LogInformation("[SUBJECT] Password Reset Request");
            logger.LogInformation("[BODY] Click here to reset: {resetLink}", resetLink);
            logger.LogInformation("==================================================================");
        }

        return Task.CompletedTask;
    }
}