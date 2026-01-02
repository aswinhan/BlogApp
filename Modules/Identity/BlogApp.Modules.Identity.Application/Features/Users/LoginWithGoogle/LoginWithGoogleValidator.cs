namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public class LoginWithGoogleValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
}
