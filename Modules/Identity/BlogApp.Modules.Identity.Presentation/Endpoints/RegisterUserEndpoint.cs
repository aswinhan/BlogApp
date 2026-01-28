namespace BlogApp.Modules.Identity.Presentation.Endpoints;

public class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            RegisterUserRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName);

            Result<Guid> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.ToProblemDetails();
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Auth")
        .WithName("RegisterUser")
        .WithSummary("Register a new user") // Replaces WithOpenApi logic
        .WithDescription("Creates a new user account and triggers email verification.")
        .Produces<Guid>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

// Helper Record for the Request Body
public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);