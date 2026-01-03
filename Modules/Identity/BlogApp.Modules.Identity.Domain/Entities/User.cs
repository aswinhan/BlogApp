namespace BlogApp.Modules.Identity.Domain.Entities;

public sealed class User : Entity, IAuditableEntity
{
    // 1. Roles Collection (Encapsulated)
    private readonly List<Role> _roles = [];

    // EF Core Requirement
    private User() { }

    // 2. Private Constructor (Enforces usage of Create Factory)
    private User(string firstName, string lastName, string email, string? passwordHash)
        : base(Guid.NewGuid())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;

        // SECURITY: Generate initial stamp
        SecurityStamp = Guid.NewGuid().ToString();

        IsEmailVerified = false;
    }

    // 3. C# 14 Properties with Built-in Sanitization
    public string FirstName
    {
        get;
        private set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("First name required");
            field = value.Trim();
        }
    }

    public string LastName
    {
        get;
        private set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Last name required");
            field = value.Trim();
        }
    }

    public string Email
    {
        get;
        private set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email required");
            // Standardize email to lower case immediately
            field = value.Trim().ToLowerInvariant();
        }
    }

    public string? PasswordHash { get; private set; }

    // 4. SECURITY STAMP (The "Unhackable" Feature)
    // Changing this invalidates ALL current login tokens for this user.
    public string SecurityStamp { get; private set; } = string.Empty;

    public bool IsEmailVerified { get; private set; }

    // 5. 2FA / MFA Support
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }

    // 6. External Auth (Google)
    public string? GoogleId { get; private set; }

    // 7. Password Reset
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiration { get; private set; }

    // 8. Audit (IAuditableEntity)
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    // 9. Factory Method
    public static User Create(string firstName, string lastName, string email, string? passwordHash)
    {
        var user = new User(firstName, lastName, email, passwordHash);

        // Default Role
        user._roles.Add(Role.User);

        // Raise Domain Event (Optional but recommended)
        // user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    // --- DOMAIN BEHAVIORS ---

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void EnableTwoFactor(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret)) throw new ArgumentException("Invalid 2FA secret");

        TwoFactorEnabled = true;
        TwoFactorSecret = secret;

        // SECURITY: Rotate stamp because auth security level changed
        SecurityStamp = Guid.NewGuid().ToString();
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void LinkGoogleAccount(string googleId)
    {
        if (string.IsNullOrWhiteSpace(googleId)) throw new ArgumentException("Invalid Google ID");

        GoogleId = googleId;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash cannot be empty");

        PasswordHash = passwordHash;

        // SECURITY CRITICAL: Rotate the stamp!
        // This ensures if a hacker had an old token, it is now USELESS.
        SecurityStamp = Guid.NewGuid().ToString();

        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SetPasswordResetToken(string token, DateTime expiration)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiration = expiration;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiration = null;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}