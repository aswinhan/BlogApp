using BlogApp.Modules.Blog.Application.Features.Articles.GetArticle;
using BlogApp.Modules.Blog.Application.Features.Articles.PublishArticle;
using BlogApp.Modules.Blog.Application.Features.Comments.AddComment;
using BlogApp.Modules.Blog.Presentation.Endpoints;
// Identity Feature DTOs
using BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;
using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;
using BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;
using BlogApp.Modules.Identity.Application.Features.Users.RegisterUser;
using BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;
using BlogApp.Modules.Identity.Presentation.Endpoints;
using System.Text.Json.Serialization;

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

// 3. Blog DTOs
[JsonSerializable(typeof(GetArticleQuery))]
[JsonSerializable(typeof(ArticleResponse))]
[JsonSerializable(typeof(PublishArticleCommand))]
[JsonSerializable(typeof(AddCommentCommand))]
[JsonSerializable(typeof(AddCommentRequest))]

public partial class AppJsonSerializerContext : JsonSerializerContext
{
}