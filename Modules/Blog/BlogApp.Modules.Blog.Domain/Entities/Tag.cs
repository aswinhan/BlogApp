namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Tag : Entity
{
    private Tag() { } // EF Core

    private Tag(string name) : base(Guid.NewGuid())
    {
        Name = name;
    }

    public string Name { get; private set; }

    // Relationship: Many-to-Many with Articles
    public List<Article> Articles { get; } = [];

    public static Tag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty");

        return new Tag(name.Trim().ToLowerInvariant());
    }
}