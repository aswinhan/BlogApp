using BlogApp.Shared.Domain.Abstractions;

namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Article : Entity, IAuditableEntity
{
    private readonly List<Tag> _tags = [];
    private readonly List<Comment> _comments = [];

    private Article() { }

    private Article(Guid authorId, string title, string content, string? summary)
        : base(Guid.NewGuid())
    {
        AuthorId = authorId;
        Title = title;
        Content = content;
        Summary = summary;
        Status = ArticleStatus.Draft;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid AuthorId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string? Summary { get; private set; }
    public ArticleStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public DateTime? PublishedOnUtc { get; private set; }

    // Relationships
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public static Article Create(Guid authorId, string title, string content, string? summary)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required");

        var article = new Article(authorId, title.Trim(), content.Trim(), summary?.Trim());

        // Raising Domain Event
        // article.RaiseDomainEvent(new ArticleCreatedDomainEvent(article.Id));

        return article;
    }

    public void Update(string title, string content, string? summary)
    {
        Title = title.Trim();
        Content = content.Trim();
        Summary = summary?.Trim();
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void Publish()
    {
        Status = ArticleStatus.Published;
        PublishedOnUtc = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void AddTag(Tag tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
        }
    }

    public void AddComment(Guid userId, string content)
    {
        var comment = Comment.Create(Id, userId, content);
        _comments.Add(comment);
    }
}