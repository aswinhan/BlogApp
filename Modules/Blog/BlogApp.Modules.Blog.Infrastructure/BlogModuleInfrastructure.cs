using BlogApp.Modules.Blog.Application.Metrics;
using BlogApp.Shared.Infrastructure.Interceptors;
using BlogApp.Shared.Infrastructure.Outbox;

namespace BlogApp.Modules.Blog.Infrastructure;

public static class BlogModuleInfrastructure
{
    public static IServiceCollection AddBlogModuleInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IModuleDatabaseMigrator, BlogModuleDatabaseMigrator>();

        // 1. Database
        // Assuming "postgres" is your connection string name or a shared helper
        // Since you used "AddPostgres", I assume it's from Aspire ServiceDefaults or Shared
        // If it's standard EF:
        var connectionString = configuration.GetConnectionString("postgres");

        services.AddDbContext<BlogDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "blog");
            })
            // Add Interceptors
            .AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<InsertOutboxMessagesInterceptor>()
            );
        });

        services.AddScoped<IBlogDbContext>(sp => sp.GetRequiredService<BlogDbContext>());


        // 2. Metrics Registration [CRITICAL FIX]
        // This ensures Minimal APIs treat BlogMetrics as a Service, not a Body
        services.AddSingleton<BlogMetrics>();

        return services;
    }
}