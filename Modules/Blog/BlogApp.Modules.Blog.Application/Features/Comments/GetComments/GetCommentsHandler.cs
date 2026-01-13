namespace BlogApp.Modules.Blog.Application.Features.Comments.GetComments;

internal sealed class GetCommentsHandler(IBlogDbContext context)
    : IQueryHandler<GetCommentsQuery, List<CommentResponse>>
{
    public async Task<Result<List<CommentResponse>>> Handle(GetCommentsQuery request, CancellationToken ct)
    {
        var comments = await context.Comments
            .AsNoTracking()
            .Where(c => c.ArticleId == request.ArticleId)
            .OrderByDescending(c => c.CreatedOnUtc) // Newest first
            .Select(c => new CommentResponse(
                c.Id,
                c.UserId,
                c.Content,
                c.CreatedOnUtc))
            .ToListAsync(ct);

        return Result.Success(comments);
    }
}