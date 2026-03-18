namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record AssignScenarioRequest(
    Guid ScenarioId,
    Guid UserId);
