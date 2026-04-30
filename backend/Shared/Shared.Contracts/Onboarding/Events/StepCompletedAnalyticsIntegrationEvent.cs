using MediatR;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record StepCompletedAnalyticsIntegrationEvent(
    Guid TenantId,
    Guid UserId,
    Guid AssignmentId,
    Guid ScenarioId,
    Guid StageId,
    Guid StepId,
    StepType StepType,
    DateTime CompletedAt,
    TimeSpan TimeSpent,
    bool WasSuccessful,
    int? Score,
    int? MaxScore,
    int AttemptNumber
) : INotification;