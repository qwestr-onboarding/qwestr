using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Events;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.CreateInvitation;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, ActionResult<Guid>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPublisher _publisher;

    public CreateInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IPublisher publisher)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
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

        var inviter = await _userRepository.GetByIdAsync(request.InvitedByUserId, ct);
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, ct);
        if (tenant == null)
        {
            return ActionResult<Guid>.Failure("Tenant not found");
        }
        var inviterName = inviter is not null
            ? $"{inviter.FirstName} {inviter.LastName}".Trim()
            : string.Empty;
        var companyName = tenant?.Name ?? string.Empty;

        await _publisher.Publish(new UserInvitedIntegrationEvent(
            invitation.TenantId,
            invitation.Email,
            inviterName,
            companyName,
            tenant!.Subdomain!,
            invitation.Token), ct);

        return ActionResult<Guid>.Success(invitation.Id);
    }
}
