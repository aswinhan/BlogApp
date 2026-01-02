using BlogApp.Modules.Identity.Application.Features.Users.LoginUser; // Reuse LoginResponse
using BlogApp.Shared.Application.Abstractions.Messaging;

namespace BlogApp.Modules.Identity.Application.Features.Users.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponse>;