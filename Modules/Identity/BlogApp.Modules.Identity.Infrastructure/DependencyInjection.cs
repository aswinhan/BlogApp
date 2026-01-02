using BlogApp.Modules.Identity.Application.Abstractions.Email;
using BlogApp.Modules.Identity.Infrastructure.Services;

namespace BlogApp.Modules.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres<IdentityDbContext>("postgres");

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        services.AddScoped<IEmailService, MockEmailService>();


        return services;
    }
}