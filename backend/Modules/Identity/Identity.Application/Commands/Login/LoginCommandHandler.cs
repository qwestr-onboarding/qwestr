using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;
using DomainRefreshToken = Qwestr.Modules.Identity.Domain.Entities.RefreshToken;

namespace Qwestr.Modules.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ActionResult<TokenResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ActionResult<TokenResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetBySubdomainAsync(request.Subdomain, ct);
        if (tenant is null || !tenant.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("Invalid credentials.");
        }

        var user = await _userRepository.GetByEmailAsync(tenant.Id, request.Email, ct);
        if (user is null || !user.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("Invalid credentials.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return ActionResult<TokenResponse>.Failure("Invalid credentials.");
        }

        if (user.TwoFactorEnabled)
        {
            var (tempToken, tempExpiresAt) = _tokenService.GenerateTwoFactorToken(user.Id);
            return ActionResult<TokenResponse>.Success(
                new TokenResponse(tempToken, string.Empty, tempExpiresAt, RequiresTwoFactor: true));
        }

        user.RecordLogin();

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var (refreshTokenValue, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

        var refreshToken = DomainRefreshToken.Create(user.Id, refreshTokenValue, refreshExpiresAt);
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult<TokenResponse>.Success(
            new TokenResponse(accessToken, refreshTokenValue, expiresAt));
    }
}
