using MediatR;

namespace Qwestr.Shared.Contracts.Gamification.Events;

public sealed record AwardEarnedIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    Guid AwardId,
    string AwardCode,
    DateTime EarnedAt) : INotification;
