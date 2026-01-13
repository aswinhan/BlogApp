namespace BlogApp.Modules.Blog.Application.Features.Tags.GetTags;

// Returns a simple list of Tag DTOs
public sealed record GetTagsQuery() : IQuery<List<TagResponse>>;

public sealed record TagResponse(Guid Id, string Name, int Count);