using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActionResult> Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.TargetUserId, ct);

        if (user is null || user.TenantId != request.TenantId)
        {
            return ActionResult.Failure("User not found.");
        }

        if (user.IsActive)
        {
            return ActionResult.Success();
        }

        user.Activate();
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
