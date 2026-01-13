namespace BlogApp.Modules.Blog.Application.Features.Comments.GetComments;

public sealed record GetCommentsQuery(Guid ArticleId) : IQuery<List<CommentResponse>>;
public sealed record CommentResponse(Guid Id, Guid UserId, string Content, DateTime CreatedOn);
