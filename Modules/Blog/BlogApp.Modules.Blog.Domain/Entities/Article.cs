namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Article : AggregateRoot, IAuditableEntity, ISoftDeletable
{
    // --- State ---
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Content { get; private set; }
    public string? Summary { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Category Category { get; set; }
    public ArticleStatus Status { get; private set; }
    public long ViewCount { get; private set; }
    public DateTime? PublishedOnUtc { get; set; }

    // Concurrency Token
    // This GUID changes every time the entity is modified.
    public Guid ConcurrencyToken { get; private set; }

    // --- Auditing ---
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    
    // --- Soft Delete ---
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    // --- Collections ---
    private readonly List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private readonly List<Comment> _comments = [];
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    // --- Constructors ---
    private Article() { } // Required by EF Core

    // --- Factory Method ---
    public static Article Create(
        string title,
        string content,
        string? summary,
        Guid authorId,
        string slug,
        Guid? categoryId)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            Summary = summary,
            AuthorId = authorId,
            Slug = slug,
            CategoryId = categoryId,
            Status = ArticleStatus.Draft,
            CreatedOnUtc = DateTime.UtcNow,
            ViewCount = 0,
            IsDeleted = false,
            ConcurrencyToken = Guid.NewGuid()
        };

        // Adds event to the internal list (Pure C#)
        article.RaiseDomainEvent(new ArticleCreatedDomainEvent(article.Id));

        return article;
    }

    // --- Lifecycle Actions ---

    public void Publish()
    {
        if (Status == ArticleStatus.Published) return;

        Status = ArticleStatus.Published;
        ModifiedOnUtc = DateTime.UtcNow;
        // Logic: Published Date usually matches the FIRST time it was published
        // If you want to update it every time, add: PublishedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ArticlePublishedDomainEvent(Id));
    }

    public void Archive()
    {
        if (Status == ArticleStatus.Archived) return;

        Status = ArticleStatus.Archived;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedOnUtc = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ArticleDeletedDomainEvent(Id));
    }

    public void Recover()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedOnUtc = null;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    // --- Updates ---

    public void UpdateDetails(string title, string content, string? summary, string slug, Guid? categoryId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug cannot be empty");

        Title = title.Trim();
        Content = content.Trim();
        Summary = summary?.Trim();
        Slug = slug;
        CategoryId = categoryId;

        ModifiedOnUtc = DateTime.UtcNow;
        ConcurrencyToken = Guid.NewGuid();

        RaiseDomainEvent(new ArticleUpdatedDomainEvent(Id));
    }

    public void UpdateCover(string? imageUrl)
    {
        CoverImageUrl = imageUrl;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    // --- Relationships ---

    public void AddTag(Tag tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            ModifiedOnUtc = DateTime.UtcNow;
        }
    }

    public void AddComment(Guid userId, string content)
    {
        var comment = Comment.Create(Id, userId, content);
        _comments.Add(comment);
        // Note: Adding a comment typically does NOT change Article.ModifiedOnUtc
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }
}