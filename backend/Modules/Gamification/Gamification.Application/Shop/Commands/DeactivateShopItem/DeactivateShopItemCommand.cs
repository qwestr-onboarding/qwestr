using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.DeactivateShopItem;

public record DeactivateShopItemCommand(Guid TenantId, Guid Id) : IRequest<ActionResult>;
