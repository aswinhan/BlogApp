namespace BlogApp.Modules.Blog.Infrastructure.Database;

internal sealed class BlogModuleDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}