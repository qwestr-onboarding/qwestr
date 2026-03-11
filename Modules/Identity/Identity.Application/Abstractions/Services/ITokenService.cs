using System.Security.Claims;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Application.Abstractions.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
