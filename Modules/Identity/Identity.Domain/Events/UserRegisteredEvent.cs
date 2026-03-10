using MediatR;

namespace Identity.Domain.Events;

public record UserRegisteredEvent(Guid UserId, Guid TenantId, string Email, string FullName) : INotification;
