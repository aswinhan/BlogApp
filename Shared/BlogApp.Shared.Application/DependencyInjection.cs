namespace BlogApp.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleAssemblies);

            // Register Pipeline Behaviors
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register Validators manually to ensure it works for all assemblies
        // This replaces "AddValidatorsFromAssemblies" which can be flaky with arrays
        foreach (var assembly in moduleAssemblies)
        {
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        }

        return services;
    }
}