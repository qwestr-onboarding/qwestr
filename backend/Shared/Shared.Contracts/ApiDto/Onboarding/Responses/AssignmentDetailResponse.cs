using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record AssignmentDetailResponse(
    Guid Id,
    Guid ScenarioId,
    string ScenarioTitle,
    Guid UserId,
    Guid AssignedBy,
    AssignmentStatus Status,
    DateTime AssignedAt,
    DateTime? CompletedAt,
    List<StepProgressSummaryResponse> StepProgresses);

public record StepProgressSummaryResponse(
    Guid Id,
    Guid StepId,
    string StepTitle,
    StepProgressStatus Status,
    DateTime? StartedAt,
    DateTime? CompletedAt);
