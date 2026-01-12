namespace BlogApp.Shared.Application.Pagination;

public record PaginationRequest(int Page = 1, int PageSize = 10)
{
    // Ensure logical limits (e.g., max 50 items per page)
    public int PageSize { get; init; } = PageSize > 50 ? 50 : PageSize;
}
