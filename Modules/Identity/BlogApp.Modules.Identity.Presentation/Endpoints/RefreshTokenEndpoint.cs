using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;
using BlogApp.Shared.Application.Abstractions.Messaging;
using BlogApp.Shared.Domain.Results;
using BlogApp.Shared.Presentation.Endpoints;
using BlogApp.Shared.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class RefreshTokenEndpoint : IEndpoint
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