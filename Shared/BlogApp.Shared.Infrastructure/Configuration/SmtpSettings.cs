namespace BlogApp.Shared.Infrastructure.Configuration;

public record SmtpSettings
{
    public const string SectionName = "SmtpSettings";

    public required string Host { get; init; }
    public int Port { get; init; }
    public required string SenderEmail { get; init; } // The "From" address
    public required string SenderName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public bool EnableSsl { get; init; }
}