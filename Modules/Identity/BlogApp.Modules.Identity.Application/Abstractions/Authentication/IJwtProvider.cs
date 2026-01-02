namespace BlogApp.Modules.Identity.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Create(User user);
}