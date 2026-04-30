using MediatR;

namespace Qwestr.Shared.Contracts.Gamification.Events;

public record PointsGrantedIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    int Amount,
    DateTime GrantedAt
) : INotification;