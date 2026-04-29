using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemsForManagement;

public record GetShopItemsForManagementQuery(Guid TenantId) : IRequest<ActionResult<ShopItemManagementListResponse>>;
