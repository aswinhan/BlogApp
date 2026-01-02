using BlogApp.Shared.Application.Abstractions.Messaging;
using BlogApp.Shared.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlogApp.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        // 1. Register Dispatcher
        services.AddScoped<ISender, InMemorySender>();

        // 2. Register Handlers & Decorate with Validation
        services.Scan(scan => scan
            .FromAssemblies(moduleAssemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // 3. Apply Decorators (Validation)
        // This wraps every ICommandHandler<T> with ValidationCommandHandler<T>
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandler<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));

        return services;
    }
}