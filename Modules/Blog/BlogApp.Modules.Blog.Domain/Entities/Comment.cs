namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Comment : Entity, IAuditableEntity, ISoftDeletable
{
    private Comment() { } // EF Core

    private Comment(Guid id, Guid articleId, Guid userId, string content) : base(id)
    {
        ArticleId = articleId;
        UserId = userId;
        Content = content;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid ArticleId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }

    // Auditing
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    // Soft Delete
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    // Factory Method
    public static Comment Create(Guid articleId, Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment cannot be empty");

        return new Comment(Guid.NewGuid(), articleId, userId, content.Trim());
    }

    // Behaviors
    public void Edit(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Comment cannot be empty");

        Content = newContent.Trim();
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedOnUtc = DateTime.UtcNow;
    }
    public void Recover() { IsDeleted = false; DeletedOnUtc = null; }

}