using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemById;

public record GetShopItemByIdQuery(Guid TenantId, Guid UserId, Guid ItemId) : IRequest<ActionResult<ShopItemResponse>>;
