using Qwestr.Modules.Analytics.Domain.Entities.Insights;

namespace Qwestr.Modules.Analytics.Application.AI.Models;

public sealed record AnalyticsSnapshot(
    PeriodInfo Period,
    OverallMetrics Overall,
    IReadOnlyList<ScenarioMetric> Scenarios,
    IReadOnlyList<ProblemStepSnapshot> ProblemSteps,
    IReadOnlyList<CohortSnapshot> RecentCohorts);

public sealed record PeriodInfo(DateOnly From, DateOnly To);

public sealed record OverallMetrics(
    int ActiveEmployees,
    int TotalAssignments,
    decimal AvgCompletionRate,
    int AvgOnboardingDurationDays,
    int AtRiskEmployees);

public sealed record ScenarioMetric(
    string ScenarioName,
    int Assigned,
    int Completed,
    decimal CompletionRate,
    int AvgDurationDays);

public sealed record ProblemStepSnapshot(
    string ScenarioName,
    string StepName,
    string StepType,
    decimal DropOffRate,
    decimal? FirstAttemptPassRate,
    int Attempts);

public sealed record CohortSnapshot(
    DateOnly WeekStart,
    int Size,
    decimal RetentionWeek1,
    decimal RetentionWeek4);

public sealed record LlmInsightResponse(
    IReadOnlyList<InsightDraft> Insights,
    IReadOnlyList<RecommendationDraft> Recommendations);
