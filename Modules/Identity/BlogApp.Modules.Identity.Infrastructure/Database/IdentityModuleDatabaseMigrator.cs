namespace BlogApp.Modules.Identity.Infrastructure.Database;

internal sealed class IdentityModuleDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        // Resolve the specific DbContext for this module
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // Run migration
        await context.Database.MigrateAsync(cancellationToken);
    }
}