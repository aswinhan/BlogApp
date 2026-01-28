namespace BlogApp.Modules.Blog.Application;

public static class BlogModuleApplication
{
    public static IServiceCollection AddBlogModuleApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Currently, we don't have specific services to register here 
        // because MediatR is registered in the Shared layer scanning this assembly.
        // But we keep this method for future Validators or specific Application Services.

        return services;
    }
}