using BlogApp.Modules.Identity.Application.Abstractions.Authentication;
using BlogApp.Modules.Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BlogApp.Modules.Identity.Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<JwtOptions> jwtOptions) : ITokenProvider
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public string CreateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // SECURITY CRITICAL: The "Kill Switch" claim
            new("security_stamp", user.SecurityStamp)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            SigningCredentials = credentials,
            Issuer = _options.Issuer,
            Audience = _options.Audience
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }

    public RefreshToken CreateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        return RefreshToken.Create(
            user.Id,
            token,
            DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays));
    }
}