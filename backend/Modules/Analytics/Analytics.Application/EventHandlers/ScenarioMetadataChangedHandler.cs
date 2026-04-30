using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Serilog;

namespace Qwestr.Modules.Analytics.Application.EventHandlers;

public class ScenarioMetadataChangedHandler : INotificationHandler<ScenarioMetadataChangedIntegrationEvent>
{
    private readonly IScenarioMetadataCacheRepository _repo;
    private readonly ILogger _logger = Log.ForContext<ScenarioMetadataChangedHandler>();

    public ScenarioMetadataChangedHandler(IScenarioMetadataCacheRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(ScenarioMetadataChangedIntegrationEvent notification, CancellationToken ct)
    {
        var existing = await _repo.FindByScenarioIdAsync(notification.TenantId, notification.ScenarioId, ct);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var stages = notification.Stages.Select(s =>
                new StageMetadataItem(
                    s.StageId,
                    s.StageName,
                    s.Order,
                    s.Steps.Select(st => new StepMetadataItem(st.StepId, st.StepName, st.Order))));

            var createResult = ScenarioMetadataCache.Create(
                notification.TenantId, notification.ScenarioId, notification.ScenarioName, stages, now);

            if (createResult.IsFailure)
            {
                _logger.Warning("ScenarioMetadataChangedHandler: Create failed: {Error}", createResult.Error);
                return;
            }

            await _repo.AddAsync(createResult.Value!, ct);
        }
        else
        {
            var stages = notification.Stages.Select(s =>
                new StageMetadataItem(
                    s.StageId,
                    s.StageName,
                    s.Order,
                    s.Steps.Select(st => new StepMetadataItem(st.StepId, st.StepName, st.Order))));

            var replaceResult = existing.Replace(notification.ScenarioName, stages, now);
            if (replaceResult.IsFailure)
            {
                _logger.Warning("ScenarioMetadataChangedHandler: Replace failed: {Error}", replaceResult.Error);
                return;
            }

            await _repo.UpdateAsync(existing, ct);
        }

        await _repo.SaveChangesAsync(ct);
    }
}
