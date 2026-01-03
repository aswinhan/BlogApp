namespace BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;

public class ForgotPasswordHandler(
    IIdentityDbContext dbContext,
    IEmailService emailService)
    : ICommandHandler<ForgotPasswordCommand>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Success();
        }

        var token = Guid.NewGuid().ToString();

        // Token valid for 1 hour
        user.SetPasswordResetToken(token, DateTime.UtcNow.AddHours(1));

        await dbContext.SaveChangesAsync(cancellationToken);

        string resetLink = $"https://localhost:7000/reset-password?token={token}&email={request.Email}";

        await emailService.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);

        return Result.Success();
    }
}