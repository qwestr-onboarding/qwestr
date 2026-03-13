using MediatR;

namespace Qwestr.Modules.Identity.Domain.Events;

public record PasswordResetRequestedEvent(Guid UserId, string Email, string Token, DateTime ExpiresAt) : INotification;
