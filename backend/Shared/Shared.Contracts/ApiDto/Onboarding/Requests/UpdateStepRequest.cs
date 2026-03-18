namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateStepRequest(
    string Title,
    int? Points,
    string? Content,
    string? Settings);
