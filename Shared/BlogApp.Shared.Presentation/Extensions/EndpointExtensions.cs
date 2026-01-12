namespace BlogApp.Shared.Presentation.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        // Find all classes that implement IApiEndpoint in the provided assemblies
        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IApiEndpoint)));

        foreach (var endpointType in endpointTypes)
        {
            services.TryAddScoped(typeof(IApiEndpoint), endpointType);
        }

        return services;
    }
    public static IApplicationBuilder MapApiEndpoints(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var endpoints = scope.ServiceProvider.GetServices<IApiEndpoint>();

        if (app is IEndpointRouteBuilder builder)
        {
            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }
        }

        return app;
    }
}