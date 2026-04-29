using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Inventory.Commands.EquipItem;

public record EquipItemCommand(Guid TenantId, Guid UserId, Guid ShopItemId) : IRequest<ActionResult>;
