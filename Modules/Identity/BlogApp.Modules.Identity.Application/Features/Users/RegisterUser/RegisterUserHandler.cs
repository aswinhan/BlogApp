// Note: We need access to the DbContext. 
// In Clean Architecture, we usually use an interface IUnitOfWork or IUserRepository.
// For Vertical Slices, injecting the DbContext directly or an abstraction is acceptable.
// We will define an IIdentityDbContext interface in Application layer to keep it clean.

namespace BlogApp.Modules.Identity.Application.Features.Users.RegisterUser;

// We need this interface first! See step below.
using BlogApp.Modules.Identity.Application.Abstractions.Data;

internal sealed class RegisterUserHandler(
    IIdentityDbContext context,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if email exists
        if (await context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Result.Failure<Guid>(Error.Conflict("User.Exists", "User with this email already exists"));
        }

        // 2. Hash password
        var passwordHash = passwordHasher.Hash(request.Password);

        // 3. Create Entity
        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash);

        // 4. Save
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        // 5. Return Id
        return user.Id;
    }
}