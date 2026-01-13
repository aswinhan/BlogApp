namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Category : Entity, IAuditableEntity, ISoftDeletable
{
    private readonly List<Article> _articles = [];

    // Private constructor for EF Core
    private Category() { }

    private Category(Guid id, string name, string slug) : base(id)
    {
        Name = name;
        Slug = slug;
    }

    public string Name { get; private set; }
    public string Slug { get; private set; }

    // Audit
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // Soft Delete
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    // Relationship
    public IReadOnlyCollection<Article> Articles => _articles.AsReadOnly();

    public static Category Create(string name, string slug)
    {
        // Add validation here
        return new Category(Guid.NewGuid(), name, slug);
    }

    // Soft Delete Methods (Implementation of ISoftDeletable)
    public void SoftDelete() { IsDeleted = true; DeletedOnUtc = DateTime.UtcNow; }
    public void Recover() { IsDeleted = false; DeletedOnUtc = null; }
}