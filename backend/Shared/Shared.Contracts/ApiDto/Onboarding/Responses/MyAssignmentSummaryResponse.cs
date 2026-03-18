using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record MyAssignmentSummaryResponse(
    Guid AssignmentId,
    Guid ScenarioId,
    string ScenarioTitle,
    string? ScenarioDescription,
    AssignmentStatus Status,
    DateTime AssignedAt,
    DateTime? CompletedAt,
    int CompletedSteps,
    int TotalSteps);
