using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Queries.ValidateInvitation;

public class ValidateInvitationQueryHandler
    : IRequestHandler<ValidateInvitationQuery, ActionResult<InvitationPreviewResponse>>
{
    private readonly IInvitationRepository _invitationRepository;

    public ValidateInvitationQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<ActionResult<InvitationPreviewResponse>> Handle(
        ValidateInvitationQuery request, CancellationToken ct)
    {
        var invitation = await _invitationRepository.GetByTokenWithTenantAsync(request.Token, ct);

        if (invitation is null)
        {
            return ActionResult<InvitationPreviewResponse>.Failure("Invitation not found.");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return ActionResult<InvitationPreviewResponse>.Failure("Invitation is no longer valid.");
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            return ActionResult<InvitationPreviewResponse>.Failure("Invitation has expired.");
        }

        return ActionResult<InvitationPreviewResponse>.Success(
            new InvitationPreviewResponse(invitation.Email, invitation.Role, invitation.Tenant.Name));
    }
}