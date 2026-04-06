using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateStepRequest(
    string Title,
    StepType Type,
    int? Points,
    string? Content,
    string? Settings);
