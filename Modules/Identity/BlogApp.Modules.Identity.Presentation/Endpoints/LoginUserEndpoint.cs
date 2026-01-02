namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class LoginUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
            [FromBody] LoginUserRequest request,
            IDispatcher dispatcher,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            Result<LoginResponse> result = await dispatcher.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            // Secure Cookie Logic
            context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Always true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // Return only the AccessToken in the body
            return Results.Ok(new { result.Value.AccessToken });
        })
        .WithTags("Auth")
        .WithSummary("Log in a user");
    }
}

// Request DTO (keep this locally or in a shared contract)
public record LoginUserRequest(string Email, string Password);