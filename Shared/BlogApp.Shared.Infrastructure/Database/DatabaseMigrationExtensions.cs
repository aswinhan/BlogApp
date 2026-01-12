namespace BlogApp.Shared.Infrastructure.Database;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrateModuleDatabasesAsync(this IServiceScope scope, CancellationToken cancellationToken = default)
    {
        // 1. Find all registered migrators
        var migrators = scope.ServiceProvider.GetServices<IModuleDatabaseMigrator>();

        // 2. Run them one by one
        foreach (var migrator in migrators)
        {
            await migrator.MigrateAsync(scope, cancellationToken);
        }
    }
}