using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Inventory.Queries.GetMyInventory;

public record GetMyInventoryQuery(Guid TenantId, Guid UserId) : IRequest<ActionResult<InventoryListResponse>>;
