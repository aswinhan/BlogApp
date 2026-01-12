namespace BlogApp.Modules.Identity.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string Create(User user);
}