namespace BlogApp.Modules.Identity.Application.Abstractions.Auth;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}