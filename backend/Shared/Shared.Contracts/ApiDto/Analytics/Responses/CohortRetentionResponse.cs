namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record CohortRetentionResponse(IReadOnlyList<CohortRowResponse> Cohorts);

public record CohortRowResponse(
    DateOnly WeekStart,
    int CohortSize,
    decimal RetentionWeek1,
    decimal RetentionWeek4);
