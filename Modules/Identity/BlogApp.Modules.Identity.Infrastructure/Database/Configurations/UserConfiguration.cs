using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlogApp.Modules.Identity.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(255);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.GoogleId)
       .IsUnique()
       .HasFilter("\"GoogleId\" IS NOT NULL"); // Postgres syntax specific (PascalCase column names)

        // Storing Roles as a simple list of Ints or Strings for now (Postgres Array) is an advanced option,
        // but let's stick to standard relations or simple JSON conversion if using Postgres.
        // For simplicity in Clean Architecture without a Join Table overhead for Enums:
        // Fixed with ValueComparer to remove EF Core Migration warnings
        builder.Property(u => u.Roles)
               .HasConversion(
                   // C# -> DB: Convert IReadOnlyCollection to comma-separated string
                   v => string.Join(',', v.Select(r => r.ToString())),

                   // DB -> C#: Convert string back to List (which implements IReadOnlyCollection)
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => Enum.Parse<Role>(s))
                         .ToList())

               // [FIX] Use the correct type for the ValueComparer
               .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<Role>>(
                   (c1, c2) => c1!.SequenceEqual(c2!),
                   c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                   c => c.ToList())); // Cloning logic
    }
}