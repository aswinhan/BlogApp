using BlogApp.Modules.Blog.Application.Features.Articles.CreateArticle;
using BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;
using BlogApp.Modules.Blog.Application.Features.Articles.GetArticles;
using BlogApp.Modules.Blog.Application.Features.Articles.GetMyArticles;
using BlogApp.Modules.Blog.Application.Features.Articles.PublishArticle;
using BlogApp.Modules.Blog.Application.Features.Categories.GetCategories;
using BlogApp.Modules.Blog.Application.Features.Comments.AddComment;
using BlogApp.Modules.Blog.Application.Features.Comments.GetComments;
using BlogApp.Modules.Blog.Application.Features.Tags.GetTags;
using BlogApp.Modules.Blog.Presentation.Endpoints;
// Identity Feature DTOs
using BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;
using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;
using BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;
using BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;
using BlogApp.Modules.Identity.Presentation.Endpoints;
using BlogApp.Shared.Domain.Pagination;

namespace BlogApp.Web.Serialization;

// 1. Auth & User DTOs
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LoginUserRequest))]
[JsonSerializable(typeof(LoginWithGoogleCommand))]
[JsonSerializable(typeof(RegisterUserRequest))]
[JsonSerializable(typeof(RefreshTokenCommand))]
[JsonSerializable(typeof(ForgotPasswordCommand))]
[JsonSerializable(typeof(ResetPasswordCommand))]

// 2. Standard Framework Types
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ValidationProblemDetails))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(List<string>))]

// 3. Blog DTOs (Articles)
[JsonSerializable(typeof(PagedList<ArticleSummaryResponse>))] // <--- Changed from PagedResult
[JsonSerializable(typeof(ArticleSummaryResponse))]
[JsonSerializable(typeof(GetArticlesEndpoint.GetArticlesRequest))]
[JsonSerializable(typeof(GetArticleQuery))]
[JsonSerializable(typeof(ArticleResponse))]
[JsonSerializable(typeof(CreateArticleResponse))]
[JsonSerializable(typeof(PublishArticleCommand))]
[JsonSerializable(typeof(CreateArticleEndpoint.CreateArticleRequest))]
[JsonSerializable(typeof(UpdateArticleEndpoint.UpdateRequest))]
[JsonSerializable(typeof(PagedList<DashboardArticleResponse>))]
[JsonSerializable(typeof(DashboardArticleResponse))]
[JsonSerializable(typeof(GetMyArticlesEndpoint.GetMyArticlesRequest))]

// 4. Blog DTOs (Comments)
[JsonSerializable(typeof(AddCommentCommand))]
[JsonSerializable(typeof(CommentResponse))]
[JsonSerializable(typeof(List<CommentResponse>))]
[JsonSerializable(typeof(CommentEndpoints.AddCommentRequest))]

// 5. Blog DTOs (Categories & Tags)
[JsonSerializable(typeof(CategoryResponse))]
[JsonSerializable(typeof(List<CategoryResponse>))]
[JsonSerializable(typeof(CategoryEndpoints.CreateCategoryRequest))]
[JsonSerializable(typeof(TagResponse))]
[JsonSerializable(typeof(List<TagResponse>))]

public partial class AppJsonSerializerContext : JsonSerializerContext
{
}