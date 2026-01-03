namespace BlogApp.Modules.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database (Preserved your cleaner 'AddPostgres' extension)
        services.AddPostgres<IdentityDbContext>("postgres");
        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        // 2. PASSWORD SECURITY [UPGRADE]
        // Switched to Argon2PasswordHasher (I will provide this class next). 
        // Standard hashing is not enough for "Unhackable" status.
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // 3. Token Management
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenProvider, TokenProvider>();

        // 4. External Services
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IEmailService, MockEmailService>();

        // 5. AUTHENTICATION & JWT [MOVED FROM PROGRAM.CS]
        // This makes the Identity Module self-contained. 
        // Program.cs no longer needs to know your secret keys.
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

        return services;
    }
}