using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Serilog;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class ScenarioCompletedHandler : INotificationHandler<OnboardingScenarioCompletedIntegrationEvent>
{
    private readonly IScenarioAssignmentFactRepository _repo;
    private readonly IDailyMetricsAccumulator _accumulator;
    private readonly ILogger _logger = Log.ForContext<ScenarioCompletedHandler>();

    public ScenarioCompletedHandler(IScenarioAssignmentFactRepository repo, IDailyMetricsAccumulator accumulator)
    {
        _repo = repo;
        _accumulator = accumulator;
    }

    public async Task Handle(OnboardingScenarioCompletedIntegrationEvent notification, CancellationToken ct)
    {
        var fact = await _repo.FindByAssignmentIdAsync(notification.TenantId, notification.AssignmentId, ct);
        if (fact is null)
        {
            return;
        }

        if (fact.CurrentStatus == Qwestr.Shared.Kernel.Enums.Onboarding.AssignmentStatus.Completed)
        {
            return;
        }

        var result = fact.MarkCompleted(notification.CompletedAt);
        if (result.IsFailure)
        {
            _logger.Warning("ScenarioCompletedHandler: MarkCompleted failed: {Error}", result.Error);
            return;
        }

        await _repo.UpdateAsync(fact, ct);
        await _repo.SaveChangesAsync(ct);

        await _accumulator.IncrementAsync(notification.TenantId, DailyMetricKind.ScenariosCompleted, 1, ct);
    }
}
