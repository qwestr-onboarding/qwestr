using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.Commands.CancelInvitation;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteInvitation;

public class DeleteInvitationCommandHandler : IRequestHandler<DeleteInvitationCommand, ActionResult>
{
    private readonly IInvitationRepository _invitationRepository;

    public DeleteInvitationCommandHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<ActionResult> Handle(DeleteInvitationCommand request, CancellationToken ct)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, ct);

        if (invitation is null || invitation.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Invitation not found.");
        }

        await _invitationRepository.RemoveAsync(invitation, ct);
        await _invitationRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
