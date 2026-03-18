using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record AssignmentListItemResponse(
    Guid Id,
    Guid ScenarioId,
    string ScenarioTitle,
    Guid UserId,
    AssignmentStatus Status,
    DateTime AssignedAt,
    DateTime? CompletedAt,
    int CompletedSteps,
    int TotalSteps);
