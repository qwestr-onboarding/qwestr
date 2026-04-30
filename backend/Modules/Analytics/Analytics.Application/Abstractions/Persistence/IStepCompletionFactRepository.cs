using Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence;

public interface IStepCompletionFactRepository
{
    Task AddAsync(StepCompletionFact fact, CancellationToken ct);
    Task<bool> ExistsAsync(Guid tenantId, Guid stepId, Guid userId, int attemptNumber, CancellationToken ct);
    Task<bool> HasUserActivityOnDateAsync(Guid tenantId, Guid userId, DateOnly date, CancellationToken ct);
    Task<IReadOnlyList<StepCompletionStats>> GetStepStatsAsync(Guid tenantId, Guid scenarioId, CancellationToken ct);
    Task<IReadOnlyList<DailyCompletionPoint>> GetCompletionTrendAsync(Guid tenantId, DateOnly from, DateOnly to, CancellationToken ct);
    Task<int> CountAtRiskEmployeesAsync(Guid tenantId, TimeSpan inactivityThreshold, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
