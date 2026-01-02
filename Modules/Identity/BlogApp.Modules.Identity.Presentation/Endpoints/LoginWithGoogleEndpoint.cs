namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class LoginWithGoogleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login/google", async (
            [FromBody] LoginWithGoogleCommand request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            Result<string> result = await sender.Send(request, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(new { AccessToken = result.Value });
        })
        .WithTags("Auth")
        .WithSummary("Login with Google")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}