using MediatR;

namespace Qwestr.Modules.Gamification.Domain.Events;

public sealed record ItemPurchasedEvent(Guid TenantId, Guid ProfileId, Guid UserId, Guid ShopItemId, int Price) : INotification;