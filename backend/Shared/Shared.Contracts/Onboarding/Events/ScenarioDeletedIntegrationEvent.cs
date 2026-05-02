using MediatR;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record ScenarioDeletedIntegrationEvent(Guid TenantId, Guid ScenarioId) : INotification;
