namespace BlogApp.Shared.Presentation.Pagination;

public record PaginatedResponse<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    bool HasNextPage);
