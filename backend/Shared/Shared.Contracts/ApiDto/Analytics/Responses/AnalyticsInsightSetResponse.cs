using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record AnalyticsInsightSetResponse(
    Guid Id,
    DateTime GeneratedAt,
    LlmProvider Provider,
    InsightTrigger Trigger,
    IReadOnlyList<InsightResponse> Insights,
    IReadOnlyList<RecommendationResponse> Recommendations);

public record InsightResponse(
    string Title,
    string Description,
    InsightSeverity Severity,
    string MetricReference);

public record RecommendationResponse(
    string Title,
    string Description,
    RecommendationCategory Category,
    int Priority);
