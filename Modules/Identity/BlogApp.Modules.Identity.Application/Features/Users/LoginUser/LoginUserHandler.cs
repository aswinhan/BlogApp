namespace BlogApp.Modules.Identity.Application.Features.Users.LoginUser;

internal sealed class LoginUserHandler(
    IIdentityDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider)
    : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Get User
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Failure("User.InvalidCredentials", "Invalid email or password"));
        }

        // 2. Check if the user has a password set (Google users might not)
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result.Failure<LoginResponse>(Error.Failure("User.InvalidCredentials", "Invalid email or password"));
        }

        // 3. Verify Password
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<LoginResponse>(Error.Failure("User.InvalidCredentials", "Invalid email or password"));
        }

        // 4. Generate Tokens
        var accessToken = tokenProvider.CreateAccessToken(user);
        var refreshToken = tokenProvider.CreateRefreshToken(user);

        // 5. Save Refresh Token to DB
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken.Token);
    }
}