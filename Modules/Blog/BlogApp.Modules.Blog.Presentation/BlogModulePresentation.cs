using BlogApp.Modules.Blog.Application;
using BlogApp.Modules.Blog.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Modules.Blog.Presentation;

public static class BlogModulePresentation
{
    public static IServiceCollection AddBlogModulePresentation(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Infrastructure
        // Pass the interface directly. The compiler should handle the conversion from ConfigurationManager.
        services.AddBlogModuleInfrastructure(configuration);

        // 2. Application
        services.AddBlogModuleApplication(configuration);

        return services;
    }

    public static WebApplication MapBlogEndpoints(this WebApplication app)
    {
        // The endpoints themselves define tags (e.g., "Articles", "Comments").
        var group = app.MapGroup("api/blog");

        // Scan for IEndpoint implementations
        var endpointTypes = typeof(BlogModulePresentation).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IEndpoint)));

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(group);
        }

        return app;
    }
}