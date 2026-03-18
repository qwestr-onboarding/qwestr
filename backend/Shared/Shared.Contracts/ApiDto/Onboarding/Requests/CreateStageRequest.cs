namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record CreateStageRequest(
    Guid ScenarioId,
    string Title,
    string? Description);
