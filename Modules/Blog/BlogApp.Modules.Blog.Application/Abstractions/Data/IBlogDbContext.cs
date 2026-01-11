using BlogApp.Modules.Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BlogApp.Modules.Blog.Application.Abstractions.Data;

public interface IBlogDbContext
{
    DbSet<Article> Articles { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Tag> Tags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}