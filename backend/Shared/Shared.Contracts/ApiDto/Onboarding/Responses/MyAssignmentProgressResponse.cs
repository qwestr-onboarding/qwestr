using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record MyAssignmentProgressResponse(
    Guid AssignmentId,
    Guid ScenarioId,
    string ScenarioTitle,
    AssignmentStatus Status,
    DateTime AssignedAt,
    DateTime? CompletedAt,
    List<StageProgressResponse> Stages);

public record StageProgressResponse(
    Guid StageId,
    string Title,
    int Order,
    List<StepProgressStatusResponse> Steps);

public record StepProgressStatusResponse(
    Guid StepId,
    Guid StepProgressId,
    string Title,
    StepType StepType,
    int Order,
    int? Points,
    StepProgressStatus Status);
