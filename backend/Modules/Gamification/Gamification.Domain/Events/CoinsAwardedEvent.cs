using MediatR;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.Events;

public sealed record CoinsAwardedEvent(Guid TenantId, Guid ProfileId, Guid UserId, int Amount, CoinTransactionSource Source) : INotification;