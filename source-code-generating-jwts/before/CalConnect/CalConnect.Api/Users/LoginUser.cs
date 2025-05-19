using CalConnect.Api.Database;
using CalConnect.Api.Users.Infrastructure;

namespace CalConnect.Api.Users;

internal sealed class LoginUser(AppDbContext context, PasswordHasher passwordHasher)
{
    public sealed record Request(string Email, string Password);

    public async Task<User> Handle(Request request)
    {
        User? user = await context.Users.GetByEmail(request.Email);

        if (user is null || user.EmailVerified)
        {
            throw new Exception("The user was not found");
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!verified)
        {
            throw new Exception("The password is incorrect");
        }

        return user;
    }
}
