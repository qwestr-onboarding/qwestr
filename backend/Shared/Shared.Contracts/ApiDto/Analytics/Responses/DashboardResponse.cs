namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record DashboardResponse(
    DashboardKpisResponse Kpis,
    IReadOnlyList<EngagementPointResponse> EngagementTrend30Days,
    IReadOnlyList<ProblemStepResponse> TopProblemSteps,
    IReadOnlyList<ScenarioSummaryResponse> ActiveScenarios,
    AnalyticsInsightSetResponse? LatestInsights);

public record DashboardKpisResponse(
    int ActiveEmployees,
    decimal AvgCompletionRate,
    TimeSpan AvgOnboardingDuration,
    int AtRiskEmployees);

public record EngagementPointResponse(DateOnly Date, int ActiveUsers, int StepsCompleted);

public record ProblemStepResponse(
    Guid StepId,
    string StepName,
    string ScenarioName,
    decimal DropOffRate,
    decimal? PassRate);

public record ScenarioSummaryResponse(
    Guid ScenarioId,
    string Name,
    int Assigned,
    int Completed,
    decimal CompletionRate);
