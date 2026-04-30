using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Serilog;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class StepCompletedHandler : INotificationHandler<StepCompletedAnalyticsIntegrationEvent>
{
    private readonly IStepCompletionFactRepository _stepFactRepo;
    private readonly IScenarioAssignmentFactRepository _assignmentFactRepo;
    private readonly IDailyMetricsAccumulator _accumulator;
    private readonly ILogger _logger = Log.ForContext<StepCompletedHandler>();

    public StepCompletedHandler(
        IStepCompletionFactRepository stepFactRepo,
        IScenarioAssignmentFactRepository assignmentFactRepo,
        IDailyMetricsAccumulator accumulator)
    {
        _stepFactRepo = stepFactRepo;
        _assignmentFactRepo = assignmentFactRepo;
        _accumulator = accumulator;
    }

    public async Task Handle(StepCompletedAnalyticsIntegrationEvent notification, CancellationToken ct)
    {
        var exists = await _stepFactRepo.ExistsAsync(
            notification.TenantId, notification.StepId, notification.UserId, notification.AttemptNumber, ct);
        if (exists)
        {
            return;
        }

        var factResult = StepCompletionFact.Create(
            notification.TenantId,
            notification.AssignmentId,
            notification.ScenarioId,
            notification.StageId,
            notification.StepId,
            notification.StepType,
            notification.UserId,
            notification.CompletedAt,
            notification.TimeSpent,
            notification.WasSuccessful,
            notification.Score,
            notification.MaxScore,
            notification.AttemptNumber);

        if (factResult.IsFailure)
        {
            _logger.Warning("StepCompletedHandler: failed to create fact: {Error}", factResult.Error);
            return;
        }

        await _stepFactRepo.AddAsync(factResult.Value!, ct);
        await _stepFactRepo.SaveChangesAsync(ct);

        if (notification.WasSuccessful)
        {
            var assignment = await _assignmentFactRepo.FindByAssignmentIdAsync(
                notification.TenantId, notification.AssignmentId, ct);

            if (assignment is not null && assignment.StartedAt is null)
            {
                var markResult = assignment.MarkStarted(notification.CompletedAt);
                if (markResult.IsSuccess)
                {
                    await _assignmentFactRepo.UpdateAsync(assignment, ct);
                    await _assignmentFactRepo.SaveChangesAsync(ct);
                }
            }

            await _accumulator.IncrementAsync(notification.TenantId, DailyMetricKind.StepsCompleted, 1, ct);
        }
    }
}
