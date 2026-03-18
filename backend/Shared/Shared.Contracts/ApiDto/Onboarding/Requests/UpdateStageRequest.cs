namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateStageRequest(
    string Title,
    string? Description);
