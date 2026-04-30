namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record ScenarioFunnelResponse(
    Guid ScenarioId,
    string ScenarioName,
    int Assigned,
    int Started,
    int Completed,
    decimal CompletionRate,
    IReadOnlyList<StageFunnelResponse> Stages);

public record StageFunnelResponse(
    Guid StageId,
    string StageName,
    int Reached,
    int Completed,
    TimeSpan AvgTimeSpent,
    IReadOnlyList<StepFunnelResponse> Steps);

public record StepFunnelResponse(
    Guid StepId,
    string StepName,
    int Reached,
    int Completed,
    decimal DropOffRate,
    TimeSpan AvgTimeSpent);
