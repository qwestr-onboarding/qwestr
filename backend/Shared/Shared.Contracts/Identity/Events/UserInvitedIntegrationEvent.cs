using MediatR;

namespace Qwestr.Shared.Contracts.Identity.Events;

public record UserInvitedIntegrationEvent(
    Guid TenantId,
    string RecipientEmail,
    string InviterName,
    string CompanyName,
    string ActivationToken) : INotification;
