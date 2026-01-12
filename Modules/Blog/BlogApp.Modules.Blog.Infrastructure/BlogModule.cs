namespace BlogApp.Modules.Blog.Infrastructure;

public static class BlogModule
{
    public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IModuleDatabaseMigrator, BlogModuleDatabaseMigrator>();

        // 1. Database
        services.AddPostgres<BlogDbContext>("postgres"); // Reusing the same connection string!
        services.AddScoped<IBlogDbContext>(sp => sp.GetRequiredService<BlogDbContext>());

        return services;
    }
}