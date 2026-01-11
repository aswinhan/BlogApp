namespace BlogApp.Shared.Infrastructure.Email;

public class MailKitEmailService(
    IOptions<SmtpSettings> smtpOptions,
    ILogger<MailKitEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _settings = smtpOptions.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to send email to {ToEmail} with subject {Subject}", toEmail, subject);

        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // Connect
            var options = _settings.EnableSsl ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None;
            await smtp.ConnectAsync(_settings.Host, _settings.Port, options, cancellationToken);

            // Authenticate (if credentials provided)
            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            }

            // Send
            var response = await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email successfully sent to {ToEmail}.", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            // In a real app, you might want to throw or queue this for retry.
            // For now, logging the failure is sufficient to prevent crashing the request.
        }
    }
}