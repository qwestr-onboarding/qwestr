using MediatR;

namespace Qwestr.Modules.Identity.Domain.Events;

public record TwoFactorEnabledEvent(Guid UserId, string Email) : INotification;
