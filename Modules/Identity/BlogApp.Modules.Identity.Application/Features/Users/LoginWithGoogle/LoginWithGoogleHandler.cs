using BlogApp.Modules.Identity.Application.Abstractions.Authentication;
using BlogApp.Modules.Identity.Application.Abstractions.Data;
using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Domain.Entities;
using BlogApp.Shared.Application.Abstractions.Messaging;
using BlogApp.Shared.Domain.Errors;
using BlogApp.Shared.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public class LoginWithGoogleHandler(
    IIdentityDbContext dbContext,
    IGoogleAuthService googleAuthService,
    ITokenProvider tokenProvider)
    : ICommandHandler<LoginWithGoogleCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await googleAuthService.ValidateAsync(request.IdToken, cancellationToken);
        if (googleUser is null)
            return Result.Failure<LoginResponse>(Error.Validation("Auth.InvalidGoogleToken", "Invalid Google ID Token.", []));

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleUser.GoogleId || u.Email == googleUser.Email, cancellationToken);

        if (user is null)
        {
            user = User.Create(googleUser.FirstName, googleUser.LastName, googleUser.Email, null);
            user.LinkGoogleAccount(googleUser.GoogleId);
            dbContext.Users.Add(user);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            user.LinkGoogleAccount(googleUser.GoogleId);
        }

        // Generate BOTH tokens
        var accessToken = tokenProvider.CreateAccessToken(user);
        var refreshToken = tokenProvider.CreateRefreshToken(user);

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken.Token);
    }
}