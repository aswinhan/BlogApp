using BlogApp.Modules.Blog.Application.Metrics;

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
        services.AddDbContext<BlogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("postgres")));

        services.AddScoped<IBlogDbContext>(sp => sp.GetRequiredService<BlogDbContext>());


        // 2. Metrics Registration [CRITICAL FIX]
        // This ensures Minimal APIs treat BlogMetrics as a Service, not a Body
        services.AddSingleton<BlogMetrics>();

        return services;
    }
}