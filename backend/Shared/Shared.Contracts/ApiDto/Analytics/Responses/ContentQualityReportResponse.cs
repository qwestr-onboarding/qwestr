using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record ContentQualityReportResponse(
    Guid ScenarioId,
    string ScenarioName,
    IReadOnlyList<StepQualityResponse> Steps);

public record StepQualityResponse(
    Guid StepId,
    string StepName,
    StepType StepType,
    int TotalAttempts,
    decimal DropOffRate,
    decimal? FirstAttemptPassRate,
    decimal AvgAttemptsCount,
    TimeSpan AvgTimeSpent,
    QualityFlag Flag);

public enum QualityFlag { Healthy, NeedsAttention, Critical }
