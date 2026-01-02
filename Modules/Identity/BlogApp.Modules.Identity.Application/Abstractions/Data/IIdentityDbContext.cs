namespace BlogApp.Modules.Identity.Application.Abstractions.Data;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}