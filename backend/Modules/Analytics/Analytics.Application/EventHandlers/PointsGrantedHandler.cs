using MediatR;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Shared.Contracts.Gamification.Events;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class PointsGrantedHandler : INotificationHandler<PointsGrantedIntegrationEvent>
{
    private readonly IDailyMetricsAccumulator _accumulator;

    public PointsGrantedHandler(IDailyMetricsAccumulator accumulator)
    {
        _accumulator = accumulator;
    }

    public async Task Handle(PointsGrantedIntegrationEvent notification, CancellationToken ct)
    {
        await _accumulator.IncrementAsync(
            notification.TenantId, DailyMetricKind.TotalPointsGranted, notification.Amount, ct);
    }
}
