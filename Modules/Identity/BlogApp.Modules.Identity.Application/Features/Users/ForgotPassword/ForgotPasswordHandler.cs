namespace BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;

public class ForgotPasswordHandler(
    IIdentityDbContext dbContext,
    IEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. Find User
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Security Note: Even if user is not found, we usually return "Ok" 
        // to prevent email enumeration attacks. But for now, we'll return generic success.
        if (user is null)
        {
            return Result.Success();
        }

        // 2. Generate Token (Random GUID or specialized token)
        var token = Guid.NewGuid().ToString();

        // 3. Update User Entity
        // Token valid for 1 hour
        user.SetPasswordResetToken(token, DateTime.UtcNow.AddHours(1));

        await dbContext.SaveChangesAsync(cancellationToken);

        // 4. Send Email
        // In a real app, this URL points to your Frontend (React) page
        string resetLink = $"https://localhost:5173/reset-password?token={token}&email={request.Email}";

        await emailService.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);

        return Result.Success();
    }
}
