using BlogApp.Shared.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace BlogApp.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, Assembly[] moduleAssemblies, IConfiguration configuration)
    {
        // 1. Configure Settings
        services.AddOptions<FileStorageSettings>()
            .BindConfiguration(FileStorageSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // 2. Register Service
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // 3. Configure SMTP Settings
        services.AddOptions<SmtpSettings>()
            .BindConfiguration(SmtpSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // 4. Register Email Service
        services.AddTransient<IEmailService, MailKitEmailService>();

        // 5. Distributed Auth Policies
        services.ConfigureOptions<AuthorizationConfigureOptions>();

        // 6. Redis Caching
        // We try to connect. If connection string is missing, we might skip or throw.
        string redisConnectionString = configuration.GetConnectionString("Redis")
            ?? "localhost:6379"; // Default fallback

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddScoped<ICachingService, RedisCachingService>();

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

        // Register Caching Decorator
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(CachingQueryHandler<,>));

        // 3. Apply Decorators
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandler<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));

        return services;
    }
}