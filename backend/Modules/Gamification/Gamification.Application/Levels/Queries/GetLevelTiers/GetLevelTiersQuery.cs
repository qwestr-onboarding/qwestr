using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Queries.GetLevelTiers;

public record GetLevelTiersQuery(Guid TenantId) : IRequest<ActionResult<LevelTierListResponse>>;
