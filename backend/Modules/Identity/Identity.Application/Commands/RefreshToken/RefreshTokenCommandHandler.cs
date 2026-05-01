using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.RefreshToken;

// Endpoint removed — refresh tokens are no longer used.
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ActionResult<TokenResponse>>
{
    public Task<ActionResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
        => Task.FromResult(ActionResult<TokenResponse>.Failure("Refresh tokens are no longer supported."));
}
