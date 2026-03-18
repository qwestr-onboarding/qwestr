using MediatR;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record OnboardingStepCompletedIntegrationEvent(
    Guid UserId,
    Guid TenantId,
    Guid AssignmentId,
    Guid StepId,
    Guid ScenarioId,
    int? PointsAwarded,
    StepType StepType,
    DateTime CompletedAt) : INotification;
