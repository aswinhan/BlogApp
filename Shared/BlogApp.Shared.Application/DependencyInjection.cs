using FluentValidation; // Ensure this is using FluentValidation
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlogApp.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        foreach (var assembly in moduleAssemblies)
        {
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        }

        return services;
    }
}