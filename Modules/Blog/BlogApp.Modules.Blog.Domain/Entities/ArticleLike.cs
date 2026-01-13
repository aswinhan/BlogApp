namespace BlogApp.Modules.Blog.Domain.Entities;

// This is a "Join Entity"
public sealed class ArticleLike
{
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }

    // Navigation Properties (Optional, but good for joins)
    // public Article Article { get; set; } = null!;
}