namespace BlogApp.Modules.Identity.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateAccessToken(User user);
    RefreshToken CreateRefreshToken(User user);
}