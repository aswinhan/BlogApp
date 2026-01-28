using BlogApp.Shared.Infrastructure.Interceptors;
using BlogApp.Shared.Infrastructure.Outbox;

namespace BlogApp.Modules.Identity.Infrastructure;

public static class IdentityModuleInfrastructure
{
    public static IServiceCollection AddIdentityModuleInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Database & Migrations
        services.AddTransient<IModuleDatabaseMigrator, IdentityModuleDatabaseMigrator>();

        // FIX: Use standard AddDbContext. 
        // Aspire maps "postgres" to the correct connection string automatically.
        var connectionString = configuration.GetConnectionString("postgres");

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
            })
            // Add Interceptors
            .AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<InsertOutboxMessagesInterceptor>()
            );
        });

        // Expose the interface for the Application layer
        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());


        // 2. PASSWORD SECURITY (Argon2)
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // 3. Token Management
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenProvider, TokenProvider>();

        // 4. External Services
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();


        // 5. AUTHENTICATION & JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
            });

        // 6. Authorization Policies
        services.AddAuthorization();

        // 7. Public API Implementations
        services.AddScoped<IUserApi, UserApi>();


        return services;
    }
}