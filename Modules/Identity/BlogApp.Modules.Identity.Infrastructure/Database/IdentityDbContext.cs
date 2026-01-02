namespace BlogApp.Modules.Identity.Infrastructure.Database;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options), IIdentityDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity"); // Module isolation schema
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}