namespace BlogApp.Shared.Infrastructure.Auth;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = (httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)) 
                              ?? throw new ApplicationException("User context is unavailable.");
            return Guid.Parse(userIdClaim.Value);
        }
    }
}