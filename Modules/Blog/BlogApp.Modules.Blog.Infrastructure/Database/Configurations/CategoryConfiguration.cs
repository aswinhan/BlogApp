namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Slug).HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Slug).IsUnique();

        // Soft Delete Filter
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationship (One Category -> Many Articles)
        builder.HasMany(c => c.Articles)
               .WithOne(a => a.Category)
               .HasForeignKey(a => a.CategoryId)
               .IsRequired(false) // Articles don't HAVE to have a category initially
               .OnDelete(DeleteBehavior.Restrict); // Don't delete articles if category is deleted
    }
}