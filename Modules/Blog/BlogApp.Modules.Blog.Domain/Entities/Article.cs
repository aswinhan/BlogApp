using BlogApp.Shared.Domain.Abstractions;
using BlogApp.Shared.Domain.Exceptions;

namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Article : Entity, IAuditableEntity, ISoftDeletable
{
    private readonly List<Tag> _tags = [];
    private readonly List<Comment> _comments = [];

    private Article() { }

    private Article(Guid id, Guid authorId, string title, string content, string? summary, string slug)
        : base(id)
    {
        AuthorId = authorId;
        Title = title;
        Content = content;
        Summary = summary;
        Slug = slug;
        Status = ArticleStatus.Draft;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid AuthorId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string? Summary { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public ArticleStatus Status { get; private set; }
    public string Slug { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public DateTime? PublishedOnUtc { get; private set; }

    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public static Article Create(Guid authorId, string title, string content, string? summary, string slug, Guid? categoryId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content is required");

        var article = new Article(Guid.NewGuid(), authorId, title, content, summary, slug);
        article.CategoryId = categoryId;
        return article;
    }

    public void Publish()
    {
        if (IsDeleted) throw new DomainException("Cannot publish a deleted article.");
        Status = ArticleStatus.Published;
        PublishedOnUtc = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = ArticleStatus.Archived;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedOnUtc = DateTime.UtcNow;
    }

    public void Recover()
    {
        IsDeleted = false;
        DeletedOnUtc = null;
    }

    public void UpdateDetails(string title, string content, string? summary, string slug, Guid? categoryId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");

        Title = title.Trim();     
        Content = content.Trim(); 
        Summary = summary?.Trim();
        Slug = slug;
        CategoryId = categoryId;

        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void UpdateCover(string? imageUrl)
    {
        CoverImageUrl = imageUrl;
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