namespace BlogApp.Modules.Identity.Application.Abstractions.Auth;

public interface ITokenProvider
{
    string CreateAccessToken(User user);
    RefreshToken CreateRefreshToken(User user);
}