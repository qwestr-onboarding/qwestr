using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;
using DomainRefreshToken = Qwestr.Modules.Identity.Domain.Entities.RefreshToken;

namespace Qwestr.Modules.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ActionResult<TokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ActionResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (existingToken is null)
        {
            return ActionResult<TokenResponse>.Failure("Invalid refresh token.");
        }

        // Token reuse detection: if already revoked, revoke the entire chain
        if (existingToken.IsRevoked)
        {
            await _refreshTokenRepository.RevokeAllByUserIdAsync(existingToken.UserId, ct);
            await _refreshTokenRepository.SaveChangesAsync(ct);
            return ActionResult<TokenResponse>.Failure("Refresh token reuse detected. Please log in again.");
        }

        if (existingToken.IsExpired)
        {
            return ActionResult<TokenResponse>.Failure("Refresh token has expired.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, ct);
        if (user is null || !user.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("User not found or inactive.");
        }

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var (newRefreshTokenValue, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

        existingToken.Revoke(newRefreshTokenValue);

        var newRefreshToken = DomainRefreshToken.Create(user.Id, newRefreshTokenValue, refreshExpiresAt);
        await _refreshTokenRepository.AddAsync(newRefreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return ActionResult<TokenResponse>.Success(
            new TokenResponse(accessToken, newRefreshTokenValue, expiresAt));
    }
}
