using System.Security.Claims;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Application.Abstractions.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, string? subdomain = null);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    (string Token, DateTime ExpiresAt) GenerateTwoFactorToken(Guid userId);
    Guid? GetUserIdFromTwoFactorToken(string token);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
