using Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence;

public interface IScenarioAssignmentFactRepository
{
    Task AddAsync(ScenarioAssignmentFact fact, CancellationToken ct);
    Task<ScenarioAssignmentFact?> FindByAssignmentIdAsync(Guid tenantId, Guid assignmentId, CancellationToken ct);
    Task UpdateAsync(ScenarioAssignmentFact fact, CancellationToken ct);
    Task<ScenarioFunnelData> GetFunnelDataAsync(Guid tenantId, Guid scenarioId, CancellationToken ct);
    Task<IReadOnlyList<CohortRetentionPoint>> GetCohortRetentionAsync(Guid tenantId, int weeks, CancellationToken ct);
    Task<IReadOnlyList<ScenarioSummaryRow>> GetActiveScenariosSummaryAsync(Guid tenantId, CancellationToken ct);
    Task<TimeSpan> GetAvgCompletionDurationAsync(Guid tenantId, CancellationToken ct);
    Task<int> CountActiveEmployeesAsync(Guid tenantId, CancellationToken ct);
    Task DeleteByScenarioIdAsync(Guid tenantId, Guid scenarioId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
