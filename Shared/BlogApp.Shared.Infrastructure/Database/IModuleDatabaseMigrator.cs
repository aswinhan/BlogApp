namespace BlogApp.Shared.Infrastructure.Database;

public interface IModuleDatabaseMigrator
{
    Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default);
}