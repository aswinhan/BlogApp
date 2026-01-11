namespace BlogApp.Modules.Blog.Application.Features.Comments.AddComment;

// Return the Guid of the new comment
public record AddCommentCommand(Guid ArticleId, Guid UserId, string Content) : ICommand<Guid>;