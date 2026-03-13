using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.SetupTwoFactor;

public class SetupTwoFactorCommandHandler : IRequestHandler<SetupTwoFactorCommand, ActionResult<TwoFactorSetupResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;

    public SetupTwoFactorCommandHandler(IUserRepository userRepository, ITotpService totpService)
    {
        _userRepository = userRepository;
        _totpService = totpService;
    }

    public async Task<ActionResult<TwoFactorSetupResponse>> Handle(SetupTwoFactorCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            return ActionResult<TwoFactorSetupResponse>.Failure("User not found.");
        }

        if (user.TwoFactorEnabled)
        {
            return ActionResult<TwoFactorSetupResponse>.Failure("Two-factor authentication is already enabled.");
        }

        var secret = _totpService.GenerateSecret();
        user.TwoFactorSecret = secret;
        await _userRepository.SaveChangesAsync(ct);

        var qrCodeUri = _totpService.GenerateQrCodeUri(user.Email, secret);

        return ActionResult<TwoFactorSetupResponse>.Success(new TwoFactorSetupResponse(secret, qrCodeUri));
    }
}
