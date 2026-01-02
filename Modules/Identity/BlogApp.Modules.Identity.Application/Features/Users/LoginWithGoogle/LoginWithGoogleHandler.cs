namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public class LoginWithGoogleHandler(
    IIdentityDbContext dbContext,
    IGoogleAuthService googleAuthService,
    IJwtProvider jwtProvider)
    : ICommandHandler<LoginWithGoogleCommand, string>
{
    public async Task<Result<string>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Token
        GoogleUserDto? googleUser = await googleAuthService.ValidateAsync(request.IdToken, cancellationToken);

        if (googleUser is null)
        {
            return Result.Failure<string>(Error.Validation("Auth.InvalidGoogleToken", "Invalid Google ID Token.", []));
        }

        // 2. Check if user exists
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleUser.GoogleId || u.Email == googleUser.Email, cancellationToken);

        if (user is null)
        {
            // CASE A: Create New User
            // FIXED ARGUMENT ORDER: (Email, PasswordHash, FirstName, LastName)
            user = User.Create(
                
                googleUser.FirstName,
                googleUser.LastName,
                googleUser.Email,
                null);

            user.LinkGoogleAccount(googleUser.GoogleId);

            dbContext.Users.Add(user);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            // CASE B: Link Existing Account
            user.LinkGoogleAccount(googleUser.GoogleId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // 3. Generate JWT
        string accessToken = jwtProvider.Create(user);

        // Explicit generic return
        return Result<string>.Success(accessToken);
    }
}