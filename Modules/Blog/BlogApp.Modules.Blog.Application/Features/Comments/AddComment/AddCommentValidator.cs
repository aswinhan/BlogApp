namespace BlogApp.Modules.Blog.Application.Features.Comments.AddComment;

public class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(c => c.Content).NotEmpty().MaximumLength(1000);
        RuleFor(c => c.ArticleId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
    }
}