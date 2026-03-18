namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateScenarioRequest(
    string Title,
    string? Description,
    List<string> Tags);
