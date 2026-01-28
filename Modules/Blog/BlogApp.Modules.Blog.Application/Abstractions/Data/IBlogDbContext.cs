namespace BlogApp.Modules.Blog.Application.Abstractions.Data;

public interface IBlogDbContext
{
    DbSet<Article> Articles { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Category> Categories { get; }
    DbSet<ArticleLike> ArticleLikes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}