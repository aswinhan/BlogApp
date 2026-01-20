namespace BlogApp.Shared.Application.Abstractions.PublicApi;

public interface IUserApi
{
    // Fetch a single user (for Article Author)
    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken ct = default);

    // Fetch multiple users at once (for Comments list - avoids N+1 problem)
    Task<List<UserResponse>> GetUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct = default);
}

// The DTO that flows between modules
public sealed record UserResponse(Guid Id, string FirstName, string LastName, string? Email);
