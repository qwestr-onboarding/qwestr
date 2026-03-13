using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Abstractions.Services;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ActionResult>
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher)
    {
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ActionResult> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token, ct);

        if (resetToken is null || !resetToken.IsValid)
        {
            return ActionResult.Failure("Invalid or expired password reset token.");
        }

        var user = await _userRepository.GetByIdAsync(resetToken.UserId, ct);
        if (user is null)
        {
            return ActionResult.Failure("User not found.");
        }

        var newHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newHash);
        resetToken.MarkAsUsed();

        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id, ct);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
