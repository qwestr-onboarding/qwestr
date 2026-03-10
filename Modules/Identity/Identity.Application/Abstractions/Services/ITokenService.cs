using System.Security.Claims;
using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
