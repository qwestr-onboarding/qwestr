namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record AiGenerationStatusResponse(
    bool CanGenerate,
    int UsedToday,
    int MaxPerDay,
    DateTime? NextAvailableAt);
