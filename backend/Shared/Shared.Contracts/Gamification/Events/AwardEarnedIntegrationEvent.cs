using MediatR;

namespace Qwestr.Shared.Contracts.Gamification.Events;

public sealed record AwardEarnedIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    Guid AwardId,
    string AwardCode,
    string AwardTitle,
    string? AwardDescription,
    DateTime EarnedAt) : INotification;
