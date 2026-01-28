namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class LoginWithGoogleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login/google", async (
            [FromBody] LoginWithGoogleCommand command,
            ISender sender,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            Result<LoginResponse> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            // Set Secure Cookie
            context.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new { result.Value.AccessToken });
        })
        .WithTags("Auth")
        .WithSummary("Login with Google")
        .Produces(StatusCodes.Status200OK);
    }
}