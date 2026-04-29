using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.UpdateShopItem;

public record UpdateShopItemCommand(
    Guid TenantId,
    Guid Id,
    string Name,
    string? Description,
    string IconUrl,
    string AssetData,
    int Price,
    int? RequiredLevel) : IRequest<ActionResult>;
