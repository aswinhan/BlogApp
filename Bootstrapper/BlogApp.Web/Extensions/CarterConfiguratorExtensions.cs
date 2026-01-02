using Carter;
using System.Reflection;

namespace BlogApp.Web.Extensions;

public static class CarterConfiguratorExtensions
{
    public static void WithModulesFromAssemblies(this CarterConfigurator configurator, params Assembly[] assemblies)
    {
        // 1. Find all types that implement ICarterModule
        var modules = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        // 2. Register them automatically
        foreach (var module in modules)
        {
            configurator.WithModules(module);
        }
    }
}