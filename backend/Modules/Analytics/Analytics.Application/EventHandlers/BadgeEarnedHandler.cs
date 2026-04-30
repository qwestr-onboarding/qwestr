using MediatR;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Shared.Contracts.Gamification.Events;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class BadgeEarnedHandler : INotificationHandler<AwardEarnedIntegrationEvent>
{
    private readonly IDailyMetricsAccumulator _accumulator;

    public BadgeEarnedHandler(IDailyMetricsAccumulator accumulator)
    {
        _accumulator = accumulator;
    }

    public async Task Handle(AwardEarnedIntegrationEvent notification, CancellationToken ct)
    {
        await _accumulator.IncrementAsync(notification.TenantId, DailyMetricKind.BadgesEarned, 1, ct);
    }
}
