using MediatR;

namespace Qwestr.Modules.Identity.Domain.Events;

public record InvitationCreatedEvent(Guid InvitationId, Guid TenantId, string Email, string Token) : INotification;
