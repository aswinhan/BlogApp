namespace BlogApp.Shared.Application.Email;

public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject line.</param>
    /// <param name="bodyHtml">The HTML content of the email body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailAsync(string toEmail, string subject, string bodyHtml, CancellationToken cancellationToken = default);
}