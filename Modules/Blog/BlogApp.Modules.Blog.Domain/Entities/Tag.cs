namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Tag : Entity, IAuditableEntity
{
    private readonly List<Article> _articles = [];

    private Tag(Guid id, string name) : base(id)
    {
        Name = name;
    }

    // Default constructor for EF Core
    private Tag() { }

    public string Name { get; private set; }

    // Relationships
    public IReadOnlyCollection<Article> Articles => _articles.AsReadOnly();

    // Auditing
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Tag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tag name cannot be empty", nameof(name));
        }

        return new Tag(Guid.NewGuid(), name.Trim().ToLowerInvariant());
    }
}