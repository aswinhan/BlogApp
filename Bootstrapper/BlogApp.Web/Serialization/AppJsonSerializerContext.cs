using System.Text.Json.Serialization;
using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Modules.Identity.Presentation.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Web.Serialization;

[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LoginUserRequest))]
[JsonSerializable(typeof(ProblemDetails))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}