using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, ActionResult<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITotpService _totpService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public VerifyTwoFactorCommandHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        ITotpService totpService,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _totpService = totpService;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<ActionResult<TokenResponse>> Handle(VerifyTwoFactorCommand request, CancellationToken ct)
    {
        var userId = _tokenService.GetUserIdFromTwoFactorToken(request.TempToken);
        if (userId is null)
        {
            return ActionResult<TokenResponse>.Failure("Недействительный или просроченный временный токен.");
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, ct);
        if (user is null || !user.IsActive)
        {
            return ActionResult<TokenResponse>.Failure("Пользователь не найден или неактивен.");
        }

        if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return ActionResult<TokenResponse>.Failure("Для этого пользователя не включена двухфакторная аутентификация.");
        }

        if (request.IsBackupCode)
        {
            var backupCodes = await _userRepository.GetActiveBackupCodesAsync(user.Id, ct);
            var matchingCode = backupCodes.FirstOrDefault(bc => _passwordHasher.Verify(request.Code, bc.CodeHash));

            if (matchingCode is null)
            {
                return ActionResult<TokenResponse>.Failure("Неверный резервный код.");
            }

            matchingCode.MarkAsUsed();
        }
        else
        {
            if (!_totpService.VerifyCode(user.TwoFactorSecret, request.Code))
            {
                return ActionResult<TokenResponse>.Failure("Неверный проверочный код.");
            }
        }

        user.RecordLogin();
        await _userRepository.SaveChangesAsync(ct);

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, ct);
        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user, tenant?.Subdomain);

        return ActionResult<TokenResponse>.Success(new TokenResponse(accessToken, expiresAt));
    }
}
