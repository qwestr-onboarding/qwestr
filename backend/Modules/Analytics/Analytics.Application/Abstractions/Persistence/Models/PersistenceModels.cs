using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;

public record StepCompletionStats(
    Guid StepId,
    int Attempts,
    int Successful,
    TimeSpan AvgTimeSpent,
    decimal? AvgScoreRatio,
    int FirstAttemptPassCount,
    int UniqueUsers);

public record DailyCompletionPoint(DateOnly Date, int StepsCompleted);

public record ScenarioFunnelData(int Assigned, int Started, int Completed);

public record CohortRetentionPoint(
    DateOnly CohortWeekStart,
    int CohortSize,
    int ActiveAfterWeek1,
    int ActiveAfterWeek4);

public record ScenarioSummaryRow(
    Guid ScenarioId,
    int Assigned,
    int Started,
    int Completed);
