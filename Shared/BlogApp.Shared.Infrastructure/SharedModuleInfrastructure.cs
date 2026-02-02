namespace BlogApp.Shared.Infrastructure;

public static class SharedModuleInfrastructure
{
    public static IServiceCollection AddSharedModuleInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string moduleName) // Kept but unused parameter warning suppressed via usage or removal if preferred
    {
        // 1. Messaging (The Engine)
        services.AddScoped<ISender, InMemorySender>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // 2. Interceptors
        services.AddScoped<AuditableEntityInterceptor>(); //Database Interceptors        
        services.AddScoped<InsertOutboxMessagesInterceptor>(); // Outbox Interceptor            

        // 3. Caching
        // AddStackExchangeRedisCache is in Microsoft.Extensions.DependencyInjection namespace
        // provided by Microsoft.Extensions.Caching.StackExchangeRedis package.
        // We need BOTH IDistributedCache (for simple stuff) AND IConnectionMultiplexer (for advanced sets/keys)
        var redisConnectionString = configuration.GetConnectionString("redis");

        // A. Register the raw connection for your RedisCachingService
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString!));

        // B. Register the standard IDistributedCache implementation (optional if you only use your custom service)
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        // C. Register your custom service
        services.AddSingleton<ICachingService, RedisCachingService>();

        // 4. Email
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddTransient<IEmailService, MailKitEmailService>();

        // 5. File Storage
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // 6. Swagger (NSwag)
        services.AddSwaggerDocumentation();

        // Log the module name to silence IDE0060 (Unused parameter)
        // or effectively use it for specific module configuration if needed later.
        _ = moduleName;

        return services;
    }
}