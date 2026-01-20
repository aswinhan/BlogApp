namespace BlogApp.Modules.Blog.Application.Features.Comments.GetComments;

internal sealed class GetCommentsHandler(IBlogDbContext context, IUserApi userApi)
    : IQueryHandler<GetCommentsQuery, List<CommentResponse>>
{
    public async Task<Result<List<CommentResponse>>> Handle(GetCommentsQuery request, CancellationToken ct)
    {
        // 1. Fetch Comments
        var comments = await context.Comments
            .AsNoTracking()
            .Where(c => c.ArticleId == request.ArticleId)
            .OrderByDescending(c => c.CreatedOnUtc)
            .ToListAsync(ct);

        if (comments.Count == 0)
        {
            // Must return a Result containing an empty list
            return Result.Success(new List<CommentResponse>());
        }

        // 2. Extract User IDs
        var userIds = comments.Select(c => c.UserId).Distinct();

        // 3. Batch Fetch Users (Cross-Module Call)
        var users = await userApi.GetUsersAsync(userIds, ct);

        // 4. Create Lookup Dictionary
        var userLookup = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        // 5. Map
        var response = comments.Select(c => new CommentResponse(
            c.Id,
            c.UserId,
            userLookup.GetValueOrDefault(c.UserId, "Unknown User"), // Map Name
            c.Content,
            c.CreatedOnUtc
        )).ToList();

        return Result.Success(response);
    }
}