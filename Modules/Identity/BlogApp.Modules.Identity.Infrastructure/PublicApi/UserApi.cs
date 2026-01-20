namespace BlogApp.Modules.Identity.Infrastructure.PublicApi;

internal sealed class UserApi(IdentityDbContext context) : IUserApi
{
    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;

        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public async Task<List<UserResponse>> GetUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct = default)
    {
        // 1. Get distinct IDs to optimize query
        var distinctIds = userIds.Distinct().ToList();

        if (distinctIds.Count == 0) return [];

        // 2. Fetch matches
        var users = await context.Users
            .AsNoTracking()
            .Where(u => distinctIds.Contains(u.Id))
            .Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email))
            .ToListAsync(ct);

        return users;
    }
}