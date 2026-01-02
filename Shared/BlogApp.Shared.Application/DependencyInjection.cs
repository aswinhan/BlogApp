namespace BlogApp.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        // Register Validators using FluentValidation
        // We iterate through all provided module assemblies (Identity, Blog, etc.)
        foreach (var assembly in moduleAssemblies)
        {
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        }

        return services;
    }
}