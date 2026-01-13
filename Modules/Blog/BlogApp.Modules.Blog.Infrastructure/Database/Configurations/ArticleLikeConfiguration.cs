namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

internal sealed class ArticleLikeConfiguration : IEntityTypeConfiguration<ArticleLike>
{
    public void Configure(EntityTypeBuilder<ArticleLike> builder)
    {
        // Composite Primary Key
        builder.HasKey(x => new { x.ArticleId, x.UserId });

        builder.ToTable("ArticleLikes");

        // Relationships
        builder.HasOne<Article>()
               .WithMany() // We can add a collection to Article if we want, but usually not needed
               .HasForeignKey(x => x.ArticleId)
               .OnDelete(DeleteBehavior.Cascade); // If Article deleted, likes are gone
    }
}