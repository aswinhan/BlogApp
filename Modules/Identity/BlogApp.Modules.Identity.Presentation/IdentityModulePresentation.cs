namespace BlogApp.Modules.Identity.Presentation;

public static class IdentityModulePresentation
{
    public static IServiceCollection AddIdentityModulePresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModuleInfrastructure(configuration);
        services.AddIdentityModuleApplication(configuration);

        // Optional: If your Endpoints need dependencies, register them here
        // services.AddScoped<LoginUserEndpoint>(); 
        // But usually Endpoints are static-like and just need method injection.

        return services;
    }

    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        // 1. Create a Route Group for Identity
        // The endpoints themselves define their own tags (e.g., "Auth").
        var group = app.MapGroup("api/identity");

        // 2. Scan and Map
        // We look for all types implementing IEndpoint in the current assembly
        var endpointTypes = typeof(IdentityModulePresentation).Assembly
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