namespace BlogApp.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        // 1. Register Dispatcher
        services.AddScoped<ISender, InMemorySender>();

        // 2. Register Handlers (FIX: Added publicOnly: false to scan 'internal' handlers)
        services.Scan(scan => scan
            .FromAssemblies(moduleAssemblies)

            // Register CommandHandlers (Void)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            // Register CommandHandlers (Result<T>)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            // Register QueryHandlers
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            // Register Validators
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // 3. Apply Decorators
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandler<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));

        return services;
    }
}