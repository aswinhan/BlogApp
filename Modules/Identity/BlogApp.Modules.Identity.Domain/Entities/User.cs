namespace BlogApp.Modules.Identity.Domain.Entities;

public class User : Entity, IAuditableEntity
{
    private readonly List<Role> _roles = [];

    private User() { } // EF Core requirement

    private User(string firstName, string lastName, string email, string? passwordHash)
        : base(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; } = string.Empty;
    public bool IsEmailVerified { get; private set; }

    // 2FA / MFA Support
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }

    // External Auth (Google)
    public string? GoogleId { get; private set; }

    // Password Reset
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiration { get; private set; }

    // Audit
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public static User Create(string firstName, string lastName, string email, string? passwordHash)
    {
        var user = new User(firstName, lastName, email, passwordHash);
        user._roles.Add(Role.User); // Default role
        // Raise Domain Event if needed: user.RaiseDomainEvent(new UserRegisteredEvent(user.Id));
        return user;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
    }

    public void EnableTwoFactor(string secret)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
    }

    public void LinkGoogleAccount(string googleId)
    {
        GoogleId = googleId;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void SetPasswordResetToken(string token, DateTime expiration)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiration = expiration;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiration = null;
    }
}