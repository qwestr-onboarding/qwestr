using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.CancelInvitation;

public class CancelInvitationCommandHandler : IRequestHandler<CancelInvitationCommand, ActionResult>
{
    private readonly IInvitationRepository _invitationRepository;

    public CancelInvitationCommandHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<ActionResult> Handle(CancelInvitationCommand request, CancellationToken ct)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, ct);

        if (invitation is null || invitation.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Invitation not found.");
        }

        if (invitation.Status != Domain.Enums.InvitationStatus.Pending)
        {
            return ActionResult.Failure("Only pending invitations can be cancelled.");
        }

        invitation.Cancel();
        await _invitationRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
