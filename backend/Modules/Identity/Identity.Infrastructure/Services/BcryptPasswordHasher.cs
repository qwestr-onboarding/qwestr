using Qwestr.Modules.Identity.Application.Abstractions.Services;

namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
