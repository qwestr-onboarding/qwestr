using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Inventory.Commands.UnequipItem;

public record UnequipItemCommand(Guid TenantId, Guid UserId, InventorySlot Slot) : IRequest<ActionResult>;
