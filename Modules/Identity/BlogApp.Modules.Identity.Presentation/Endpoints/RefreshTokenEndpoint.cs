namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class RefreshTokenEndpoint : IApiEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh", async (
            ISender sender,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            if (!context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return Results.Problem("Missing refresh token", statusCode: 401);
            }

            var command = new RefreshTokenCommand(refreshToken);
            Result<LoginResponse> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                context.Response.Cookies.Delete("refreshToken");
                return result.ToProblemDetails();
            }

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
        .WithSummary("Refresh Access Token");
    }
}