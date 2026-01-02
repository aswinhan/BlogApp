namespace BlogApp.Shared.Presentation.Extensions;

public static class EndpointExtensions
{
    // 1. Register all IEndpoints in DI
    public static IServiceCollection AddEndpoints(this IServiceCollection services, params Assembly[] assemblies)
    {
        var endpoints = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var endpoint in endpoints)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpoint), endpoint));
        }

        return services;
    }

    // 2. Map them at startup
    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        // Create a versioned group if needed, or just map directly
        IEndpointRouteBuilder builder = app;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}