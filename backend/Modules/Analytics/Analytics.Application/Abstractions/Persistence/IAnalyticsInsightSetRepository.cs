using Qwestr.Modules.Analytics.Domain.Entities.Insights;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence;

public interface IAnalyticsInsightSetRepository
{
    Task AddAsync(AnalyticsInsightSet set, CancellationToken ct);
    Task<AnalyticsInsightSet?> GetLatestAsync(Guid tenantId, CancellationToken ct);
    Task<DateTime?> GetLastManualGeneratedAtAsync(Guid tenantId, CancellationToken ct);
    Task<AnalyticsInsightSet?> FindByDataHashAsync(Guid tenantId, string snapshotHash, TimeSpan freshness, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
