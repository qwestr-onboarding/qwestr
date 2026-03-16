using MediatR;

namespace Qwestr.Shared.Contracts.Identity.Events;

public record UserDeactivatedIntegrationEvent(Guid UserId, Guid TenantId) : INotification;