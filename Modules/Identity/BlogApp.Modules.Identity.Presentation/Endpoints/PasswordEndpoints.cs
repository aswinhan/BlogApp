namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class PasswordEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("users").WithTags("Auth");

        // 1. Forgot Password
        group.MapPost("forgot-password", async (
            [FromBody] ForgotPasswordCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            Result result = await sender.Send(request, cancellationToken);

            // Always return OK to prevent email enumeration
            return Results.Ok();
        });

        // 2. Reset Password
        group.MapPost("reset-password", async (
            [FromBody] ResetPasswordCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            Result result = await sender.Send(request, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok();
        });
    }
}