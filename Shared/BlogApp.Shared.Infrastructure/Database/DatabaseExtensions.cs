namespace BlogApp.Shared.Infrastructure.Database;

public static class DatabaseExtensions
{
    // This method will be called by each Module (Identity, Blog, etc.)
    public static IServiceCollection AddPostgres<TDbContext>(this IServiceCollection services, string connectionName)
        where TDbContext : DbContext
    {
        // Register the Interceptor
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            // In Aspire, we will get the connection string via the configuration
            // The connectionName will be "postgres" (the name we gave in AppHost)
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(connectionName);

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"); // Optional: Keep schema clean
            })
            .AddInterceptors(interceptor);
        });

        return services;
    }
}