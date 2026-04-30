using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Application.AI.Abstractions;

public interface ILlmClient
{
    LlmProvider Provider { get; }
    Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(AnalyticsSnapshot snapshot, CancellationToken ct);
}
