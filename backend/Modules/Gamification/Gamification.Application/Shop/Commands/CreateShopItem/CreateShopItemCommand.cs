using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.CreateShopItem;

public record CreateShopItemCommand(
    Guid TenantId,
    InventorySlot Slot,
    string Name,
    string? Description,
    string IconUrl,
    string AssetData,
    int Price,
    int? RequiredLevel,
    StackingPolicy StackingPolicy) : IRequest<ActionResult<Guid>>;
