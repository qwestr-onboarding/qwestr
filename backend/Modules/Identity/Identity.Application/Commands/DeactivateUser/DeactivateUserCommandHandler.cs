using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActionResult> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.TargetUserId, ct);

        if (user is null || user.TenantId != request.TenantId)
        {
            return ActionResult.Failure("User not found.");
        }

        if (!user.IsActive)
        {
            return ActionResult.Success();
        }

        user.Deactivate();
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
