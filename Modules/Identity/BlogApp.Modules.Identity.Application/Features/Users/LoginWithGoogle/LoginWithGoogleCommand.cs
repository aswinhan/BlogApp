namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public record LoginWithGoogleCommand(string IdToken) : ICommand<string>;