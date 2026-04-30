using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Serilog;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class ScenarioAssignedHandler : INotificationHandler<OnboardingScenarioAssignedIntegrationEvent>
{
    private readonly IScenarioAssignmentFactRepository _repo;
    private readonly IDailyMetricsAccumulator _accumulator;
    private readonly ILogger _logger = Log.ForContext<ScenarioAssignedHandler>();

    public ScenarioAssignedHandler(IScenarioAssignmentFactRepository repo, IDailyMetricsAccumulator accumulator)
    {
        _repo = repo;
        _accumulator = accumulator;
    }

    public async Task Handle(OnboardingScenarioAssignedIntegrationEvent notification, CancellationToken ct)
    {
        var existing = await _repo.FindByAssignmentIdAsync(notification.TenantId, notification.AssignmentId, ct);
        if (existing is not null)
        {
            return;
        }

        var factResult = ScenarioAssignmentFact.Create(
            notification.TenantId,
            notification.AssignmentId,
            notification.ScenarioId,
            notification.UserId,
            notification.AssignedAt);

        if (factResult.IsFailure)
        {
            _logger.Warning("ScenarioAssignedHandler: failed to create fact: {Error}", factResult.Error);
            return;
        }

        await _repo.AddAsync(factResult.Value!, ct);
        await _repo.SaveChangesAsync(ct);

        await _accumulator.IncrementAsync(notification.TenantId, DailyMetricKind.ScenariosStarted, 1, ct);
    }
}
