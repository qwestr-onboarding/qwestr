using MediatR;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record OnboardingScenarioCompletedIntegrationEvent(
    Guid UserId,
    Guid TenantId,
    Guid AssignmentId,
    Guid ScenarioId,
    DateTime CompletedAt) : INotification;
