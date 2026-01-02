using BlogApp.Modules.Identity.Application.Features.Users.LoginUser;
using BlogApp.Shared.Application.Abstractions.Messaging;

namespace BlogApp.Modules.Identity.Application.Features.Users.LoginWithGoogle;

public record LoginWithGoogleCommand(string IdToken) : ICommand<LoginResponse>;