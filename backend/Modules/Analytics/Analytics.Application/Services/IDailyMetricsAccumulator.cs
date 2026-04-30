namespace Qwestr.Modules.Analytics.Application.Services;

public interface IDailyMetricsAccumulator
{
    Task IncrementAsync(Guid tenantId, DailyMetricKind kind, int delta, CancellationToken ct);
}

public enum DailyMetricKind
{
    StepsCompleted,
    ScenariosStarted,
    ScenariosCompleted,
    BadgesEarned,
    TotalPointsGranted,
    ActiveUsers
}
