namespace BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;

internal sealed class RefreshTokenHandler(
    IIdentityDbContext context,
    ITokenProvider tokenProvider)
    : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the Refresh Token (Optimized Query)
        var refreshTokenEntity = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        // 2. Validate Token Existence and Expiry
        if (refreshTokenEntity is null ||
            refreshTokenEntity.ExpiresOnUtc < DateTime.UtcNow ||
            refreshTokenEntity.RevokedOnUtc is not null)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidToken", "Invalid or expired refresh token"));
        }

        // 3. Revoke the OLD token (Rotation Security)
        // This prevents Replay Attacks. If a hacker stole the old token, they can't use it again.
        refreshTokenEntity.Revoke(DateTime.UtcNow);

        // 4. Get User
        var user = await context.Users.FindAsync([refreshTokenEntity.UserId], cancellationToken);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.UserNotFound", "User not found"));
        }

        // 5. Generate NEW Tokens
        var newAccessToken = tokenProvider.CreateAccessToken(user);
        var newRefreshToken = tokenProvider.CreateRefreshToken(user);

        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(newAccessToken, newRefreshToken.Token);
    }
}