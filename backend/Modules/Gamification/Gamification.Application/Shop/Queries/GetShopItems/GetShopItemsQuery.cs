using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItems;

public record GetShopItemsQuery(Guid TenantId, Guid UserId) : IRequest<ActionResult<ShopItemsList>>;
