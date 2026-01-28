
using BlogApp.Shared.Infrastructure.Outbox;

namespace BlogApp.Modules.Blog.Infrastructure.Database;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options), IBlogDbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ArticleLike> ArticleLikes { get; set; }

    // The Outbox Table
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
        // Apply the Shared Outbox Configuration
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}