using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.Onboarding.Events;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class ScenarioDeletedHandler : INotificationHandler<ScenarioDeletedIntegrationEvent>
{
    private readonly IScenarioAssignmentFactRepository _assignmentRepo;
    private readonly IStepCompletionFactRepository _stepRepo;
    private readonly IScenarioMetadataCacheRepository _metaRepo;

    public ScenarioDeletedHandler(
        IScenarioAssignmentFactRepository assignmentRepo,
        IStepCompletionFactRepository stepRepo,
        IScenarioMetadataCacheRepository metaRepo)
    {
        _assignmentRepo = assignmentRepo;
        _stepRepo = stepRepo;
        _metaRepo = metaRepo;
    }

    public async Task Handle(ScenarioDeletedIntegrationEvent notification, CancellationToken ct)
    {
        await _assignmentRepo.DeleteByScenarioIdAsync(notification.TenantId, notification.ScenarioId, ct);
        await _stepRepo.DeleteByScenarioIdAsync(notification.TenantId, notification.ScenarioId, ct);
        await _metaRepo.DeleteByScenarioIdAsync(notification.TenantId, notification.ScenarioId, ct);
    }
}
