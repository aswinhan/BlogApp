namespace BlogApp.Shared.Infrastructure.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        // NSwag Registration
        services.AddOpenApiDocument(config =>
        {
            config.Title = "BlogApp API";
            config.Version = "v1";
            config.Description = "Modular Monolith API";

            // 1. Define JWT Security
            config.AddSecurity("Bearer", [], new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token."
            });

            // 2. Apply Security Globally (Authorized Padlock)
            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        // NSwag Middleware
        app.UseOpenApi();       // Serves /swagger/v1/swagger.json
        app.UseSwaggerUi();     // Serves the UI at /swagger

        return app;
    }
}