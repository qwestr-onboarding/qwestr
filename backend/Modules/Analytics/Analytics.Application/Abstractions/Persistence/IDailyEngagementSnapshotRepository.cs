using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence;

public interface IDailyEngagementSnapshotRepository
{
    Task<DailyEngagementSnapshot?> FindByDateAsync(Guid tenantId, DateOnly date, CancellationToken ct);
    Task AddAsync(DailyEngagementSnapshot snapshot, CancellationToken ct);
    Task UpdateAsync(DailyEngagementSnapshot snapshot, CancellationToken ct);
    Task<IReadOnlyList<DailyEngagementSnapshot>> GetRangeAsync(Guid tenantId, DateOnly from, DateOnly to, CancellationToken ct);
    Task<IReadOnlyList<Guid>> GetActiveTenantIdsAsync(DateOnly since, CancellationToken ct);
    Task<int> CountDistinctActiveUsersAsync(Guid tenantId, DateOnly date, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
