namespace BlogApp.Modules.Blog.Infrastructure.Database.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);

        // Unique Index: Cannot have two tags with the same name
        builder.HasIndex(t => t.Name).IsUnique();
    }
}