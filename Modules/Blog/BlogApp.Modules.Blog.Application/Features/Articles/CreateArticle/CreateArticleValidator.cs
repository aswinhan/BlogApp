namespace BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;

public class CreateArticleValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Content).NotEmpty();
        RuleFor(c => c.Summary).MaximumLength(500);
        RuleFor(c => c.Tags).Must(t => t.Count <= 5).WithMessage("Maximum 5 tags allowed.");
    }
}