using MediatR;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record OnboardingScenarioAssignedIntegrationEvent(
    Guid UserId,
    Guid TenantId,
    Guid AssignmentId,
    Guid ScenarioId,
    string ScenarioTitle,
    Guid AssignedByUserId,
    DateTime AssignedAt) : INotification;
