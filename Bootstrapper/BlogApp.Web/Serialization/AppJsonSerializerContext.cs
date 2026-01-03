using System.Text.Json.Serialization;

// Identity Feature DTOs
using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;
using BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;
using BlogApp.Modules.Identity.Application.Features.Users.RegisterUser;
using BlogApp.Modules.Identity.Application.Features.Users.ForgotPassword;
using BlogApp.Modules.Identity.Application.Features.Users.ResetPassword;
using BlogApp.Modules.Identity.Presentation.Endpoints;

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

public partial class AppJsonSerializerContext : JsonSerializerContext
{
}