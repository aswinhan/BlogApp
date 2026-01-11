namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content).HasMaxLength(1000);

        // Index for performance: Quickly load all comments for an article
        builder.HasIndex(c => c.ArticleId);
    }
}