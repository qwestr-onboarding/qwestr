using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.PurchaseShopItem;

public record PurchaseShopItemCommand(Guid TenantId, Guid UserId, Guid ShopItemId) : IRequest<ActionResult>;
