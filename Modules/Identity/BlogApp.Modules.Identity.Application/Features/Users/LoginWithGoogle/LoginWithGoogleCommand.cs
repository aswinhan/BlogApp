namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public record LoginWithGoogleCommand(string IdToken) : IRequest<Result<string>>;
