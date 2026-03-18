namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record CreateScenarioRequest(
    string Title,
    string? Description,
    List<string> Tags);
