using MediatR;

namespace Qwestr.Modules.Gamification.Domain.Events;

public sealed record PlayerLevelUpEvent(Guid TenantId, Guid ProfileId, Guid UserId, int OldLevel, int NewLevel) : INotification;