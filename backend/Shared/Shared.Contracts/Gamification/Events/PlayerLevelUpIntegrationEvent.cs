using MediatR;

namespace Qwestr.Shared.Contracts.Gamification.Events;

public sealed record PlayerLevelUpIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    int OldLevel,
    int NewLevel,
    DateTime OccurredAt) : INotification;
