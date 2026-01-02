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
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // High Performance: Disables change tracking by default.
        // You must explicitly call .Update() or .Add() to save changes, which is cleaner CQRS.
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}