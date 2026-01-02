using BlogApp.Modules.Identity.Application.Abstractions.Authentication; // For IPasswordHasher
using BlogApp.Modules.Identity.Application.Abstractions.Data;
using BlogApp.Shared.Application.Abstractions.Messaging; // <--- Import Custom Interfaces
using BlogApp.Shared.Domain.Errors;
using BlogApp.Shared.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;

public class ResetPasswordHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher)
    : ICommandHandler<ResetPasswordCommand> // <--- Fixed Interface
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound("User.NotFound", "The user does not exist."));
        }

        if (user.PasswordResetToken != request.Token ||
            !user.PasswordResetTokenExpiration.HasValue ||
            user.PasswordResetTokenExpiration.Value < DateTime.UtcNow)
        {
            return Result.Failure(Error.Conflict("User.InvalidToken", "Invalid or expired reset token."));
        }

        string passwordHash = passwordHasher.Hash(request.NewPassword);

        user.UpdatePassword(passwordHash);

        user.ClearPasswordResetToken();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}