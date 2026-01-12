namespace BlogApp.Shared.Infrastructure;

public static class SharedModuleInfrastructure
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        Assembly[] moduleAssemblies,
        IConfiguration configuration)
    {
        // 1. Core Services (File, Email, Swagger)
        services.AddSwaggerWithJwt();

        services.AddOptions<FileStorageSettings>()
            .BindConfiguration(FileStorageSettings.SectionName)
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddOptions<SmtpSettings>()
            .BindConfiguration(SmtpSettings.SectionName)
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddTransient<IEmailService, MailKitEmailService>();

        // 2. Auth & Caching
        services.ConfigureOptions<AuthorizationConfigureOptions>();

        string redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConn));
        services.AddScoped<ICachingService, RedisCachingService>();

        // 3. Mediator (Scrutor Scanning)
        services.AddScoped<ISender, InMemorySender>();

        services.Scan(scan => scan
            .FromAssemblies(moduleAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces().WithScopedLifetime()
        );

        // 4. Decorators
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(CachingQueryHandler<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandler<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandler<,>));

        return services;
    }
}