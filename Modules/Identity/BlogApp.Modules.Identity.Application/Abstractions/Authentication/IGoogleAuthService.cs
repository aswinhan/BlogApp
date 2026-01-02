namespace BlogApp.Modules.Identity.Application.Abstractions.Authentication;

public interface IGoogleAuthService
{
    // Now returns our clean DTO instead of the external library type
    Task<GoogleUserDto?> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
}

public record GoogleUserDto(
    string GoogleId,
    string Email,
    string FirstName,
    string LastName);