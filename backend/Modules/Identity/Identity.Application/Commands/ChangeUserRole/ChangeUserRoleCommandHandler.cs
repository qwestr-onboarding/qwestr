using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, ActionResult>
{
    private readonly IUserRepository _userRepository;

    public ChangeUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActionResult> Handle(ChangeUserRoleCommand request, CancellationToken ct)
    {
        if (request.CurrentUserId == request.TargetUserId)
        {
            return ActionResult.Failure("You cannot change your own role.");
        }

        var user = await _userRepository.GetByIdAsync(request.TargetUserId, ct);

        if (user is null || user.TenantId != request.TenantId)
        {
            return ActionResult.Failure("User not found.");
        }

        if (user.Role == UserRole.Owner)
        {
            return ActionResult.Failure("The Owner role cannot be reassigned.");
        }

        if (request.NewRole == UserRole.Owner)
        {
            return ActionResult.Failure("Cannot assign the Owner role.");
        }

        user.ChangeRole(request.NewRole);
        await _userRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
