namespace BlogApp.Shared.Infrastructure.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddPostgres<TDbContext>(
        this IServiceCollection services,
        string connectionName,
        string schema = "public")
        where TDbContext : DbContext
    {
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(connectionName);

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // Isolate module tables into their own schema
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
            })
            .AddInterceptors(interceptor);
            //.UseSnakeCaseNamingConvention(); // Recommended for Postgres
        });

        return services;
    }
}