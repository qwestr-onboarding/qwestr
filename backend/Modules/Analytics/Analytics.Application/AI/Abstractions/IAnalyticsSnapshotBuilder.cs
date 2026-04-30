using Qwestr.Modules.Analytics.Application.AI.Models;

namespace Qwestr.Modules.Analytics.Application.AI.Abstractions;

public interface IAnalyticsSnapshotBuilder
{
    Task<AnalyticsSnapshot> BuildAsync(Guid tenantId, CancellationToken ct);
}
