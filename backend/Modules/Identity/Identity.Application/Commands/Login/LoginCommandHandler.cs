using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ActionResult<TokenResponse>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ActionResult<TokenResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetWithSettingsBySubdomainAsync(request.Subdomain, ct);
        if (tenant is null || !tenant.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("Неверные данные для входа.");
        }

        var user = await _userRepository.GetByEmailAsync(tenant.Id, request.Email, ct);
        if (user is null || !user.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("Неверные данные для входа.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return ActionResult<TokenResponse>.Failure("Неверные данные для входа.");
        }

        if (user.TwoFactorEnabled)
        {
            var (tempToken, tempExpiresAt) = _tokenService.GenerateTwoFactorToken(user.Id);
            return ActionResult<TokenResponse>.Success(
                new TokenResponse(tempToken, tempExpiresAt, RequiresTwoFactor: true));
        }

        user.RecordLogin();
        await _userRepository.SaveChangesAsync(ct);

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user, tenant.Subdomain, tenant.Settings?.Timezone);
        return ActionResult<TokenResponse>.Success(new TokenResponse(accessToken, expiresAt));
    }
}
