using BlogApp.Modules.Identity.Application.Abstractions.Auth;

namespace BlogApp.Modules.Identity.Infrastructure.Auth;

public class GoogleAuthService(
    IConfiguration configuration,
    ILogger<GoogleAuthService> logger) : IGoogleAuthService
{
    public async Task<GoogleUserDto?> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [configuration["Authentication:Google:ClientId"]]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            // Map library type to our Application DTO
            return new GoogleUserDto(
                payload.Subject,
                payload.Email,
                payload.GivenName ?? "User",
                payload.FamilyName ?? "Name"
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to validate Google Token.");
            return null;
        }
    }
}