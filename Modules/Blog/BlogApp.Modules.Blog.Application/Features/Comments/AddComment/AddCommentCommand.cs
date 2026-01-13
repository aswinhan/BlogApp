namespace BlogApp.Modules.Blog.Application.Features.Comments.AddComment;

// We return the CommentID so the UI can highlight it
public sealed record AddCommentCommand(Guid ArticleId, string Content) : ICommand<Guid>;