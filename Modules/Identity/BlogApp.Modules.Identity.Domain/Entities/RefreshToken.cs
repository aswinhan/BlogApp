namespace BlogApp.Modules.Identity.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }

    // Private ctor for EF
    private RefreshToken()
    {
        Token = string.Empty;
    }

    private RefreshToken(Guid userId, string token, DateTime expiresOnUtc)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        Token = token;
        ExpiresOnUtc = expiresOnUtc;
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresOnUtc)
    {
        return new RefreshToken(userId, token, expiresOnUtc);
    }

    public void Revoke(DateTime revokedOnUtc)
    {
        if (RevokedOnUtc is not null) return;
        RevokedOnUtc = revokedOnUtc;
    }

    public bool IsActive => RevokedOnUtc is null && DateTime.UtcNow < ExpiresOnUtc;
}