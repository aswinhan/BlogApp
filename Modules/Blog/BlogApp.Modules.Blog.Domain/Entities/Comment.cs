using BlogApp.Shared.Domain.Abstractions;

namespace BlogApp.Modules.Blog.Domain.Entities;

public sealed class Comment : Entity, IAuditableEntity
{
    private Comment() { }

    private Comment(Guid articleId, Guid userId, string content) : base(Guid.NewGuid())
    {
        ArticleId = articleId;
        UserId = userId;
        Content = content;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid ArticleId { get; private set; }
    public Guid UserId { get; private set; } // We store ID only, no navigation to Identity module!

    public string Content { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    public static Comment Create(Guid articleId, Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment cannot be empty");

        return new Comment(articleId, userId, content.Trim());
    }

    public void Update(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment cannot be empty");

        Content = content.Trim();
        ModifiedOnUtc = DateTime.UtcNow;
    }
}