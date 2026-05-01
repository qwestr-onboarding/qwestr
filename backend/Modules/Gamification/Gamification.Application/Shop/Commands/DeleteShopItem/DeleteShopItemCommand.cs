using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.DeleteShopItem;

public record DeleteShopItemCommand(Guid TenantId, Guid ItemId) : IRequest<ActionResult>;
