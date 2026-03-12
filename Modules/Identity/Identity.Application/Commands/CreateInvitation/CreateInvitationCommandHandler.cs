using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.CreateInvitation;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, ActionResult<Guid>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPublisher _publisher;

    public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IPublisher publisher)
    {
        _invitationRepository = invitationRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult<Guid>> Handle(CreateInvitationCommand request, CancellationToken ct)
    {
        var alreadyPending = await _invitationRepository.ExistsPendingAsync(request.TenantId, request.Email, ct);
        if (alreadyPending)
        {
            return ActionResult<Guid>.Failure("An active invitation for this email already exists.");
        }

        var token = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var invitation = Invitation.Create(request.TenantId, request.Email, request.Role, request.InvitedByUserId, token, expiresAt);

        await _invitationRepository.AddAsync(invitation, ct);
        await _invitationRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new InvitationCreatedEvent(invitation.Id, invitation.TenantId, invitation.Email, invitation.Token), ct);

        return ActionResult<Guid>.Success(invitation.Id);
    }
}
