namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content).HasMaxLength(1000).IsRequired();

        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationship (Already defined in ArticleConfiguration, but good to reinforce)
        builder.HasOne<Article>()
               .WithMany(a => a.Comments)
               .HasForeignKey(c => c.ArticleId)
               .OnDelete(DeleteBehavior.Cascade); // If Article is deleted, comments go too.
    }
}