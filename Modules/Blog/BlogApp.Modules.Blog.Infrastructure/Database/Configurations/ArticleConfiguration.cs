namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).HasMaxLength(200);
        builder.Property(a => a.Summary).HasMaxLength(500);
        builder.Property(a => a.Content).HasColumnType("text"); // Unlimited text

        // Enum stored as Integer (Fast) or String (Readable). 
        // Let's use string for clarity in the DB.
        builder.Property(a => a.Status)
               .HasConversion<string>();

        // Relationship: Article -> Comments (1 to Many)
        builder.HasMany(a => a.Comments)
               .WithOne()
               .HasForeignKey(c => c.ArticleId)
               .OnDelete(DeleteBehavior.Cascade); // Delete article = delete comments

        // Relationship: Article -> Tags (Many to Many)
        builder.HasMany(a => a.Tags)
               .WithMany(t => t.Articles)
               .UsingEntity(j => j.ToTable("ArticleTags")); // EF manages the join table automatically
    }
}