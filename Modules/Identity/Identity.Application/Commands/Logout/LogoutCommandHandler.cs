using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ActionResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ActionResult> Handle(LogoutCommand request, CancellationToken ct)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (token is null || token.IsRevoked)
        {
            return ActionResult.Success();
        }

        token.Revoke();
        await _refreshTokenRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
