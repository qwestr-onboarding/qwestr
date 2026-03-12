using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DisableTwoFactor;

public class DisableTwoFactorCommandHandler : IRequestHandler<DisableTwoFactorCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly IPasswordHasher _passwordHasher;

    public DisableTwoFactorCommandHandler(
        IUserRepository userRepository,
        ITotpService totpService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _totpService = totpService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ActionResult> Handle(DisableTwoFactorCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            return ActionResult.Failure("User not found.");
        }

        if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return ActionResult.Failure("Two-factor authentication is not enabled.");
        }

        var codeValid = _totpService.VerifyCode(user.TwoFactorSecret, request.Code);

        if (!codeValid)
        {
            var backupCodes = await _userRepository.GetActiveBackupCodesAsync(user.Id, ct);
            var matchingCode = backupCodes.FirstOrDefault(bc => _passwordHasher.Verify(request.Code, bc.CodeHash));

            if (matchingCode is null)
            {
                return ActionResult.Failure("Invalid code.");
            }

            matchingCode.MarkAsUsed();
        }

        user.DisableTwoFactor();
        await _userRepository.DeleteAllBackupCodesAsync(user.Id, ct);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
