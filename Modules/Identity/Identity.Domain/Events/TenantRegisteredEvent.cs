using MediatR;

namespace Identity.Domain.Events;

public record TenantRegisteredEvent(Guid TenantId, string Name) : INotification;
