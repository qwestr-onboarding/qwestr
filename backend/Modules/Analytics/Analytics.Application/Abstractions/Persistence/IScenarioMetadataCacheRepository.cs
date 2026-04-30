using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence;

public interface IScenarioMetadataCacheRepository
{
    Task<ScenarioMetadataCache?> FindByScenarioIdAsync(Guid tenantId, Guid scenarioId, CancellationToken ct);
    Task AddAsync(ScenarioMetadataCache cache, CancellationToken ct);
    Task UpdateAsync(ScenarioMetadataCache cache, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, string>> GetStepNamesAsync(Guid tenantId, IEnumerable<Guid> stepIds, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, string>> GetScenarioNamesAsync(Guid tenantId, IEnumerable<Guid> scenarioIds, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
