using MediatR;

namespace Qwestr.Modules.Identity.Domain.Events;

public record TenantRegisteredEvent(Guid TenantId, string Name) : INotification;
