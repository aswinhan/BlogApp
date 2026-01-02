namespace BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;

public class ResetPasswordHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Find User
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound("User.NotFound", "The user does not exist."));
        }

        // 2. Validate Token
        if (user.PasswordResetToken != request.Token ||
            !user.PasswordResetTokenExpiration.HasValue ||
            user.PasswordResetTokenExpiration.Value < DateTime.UtcNow)
        {
            return Result.Failure(Error.Conflict("User.InvalidToken", "Invalid or expired reset token."));
        }

        // 3. HASH THE PASSWORD CORRECTLY
        // The passwordHasher will return a string like "$2a$11$..."
        string passwordHash = passwordHasher.Hash(request.NewPassword);

        // 4. Update User
        user.UpdatePassword(passwordHash);

        // 5. Clear Token
        user.ClearPasswordResetToken();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}