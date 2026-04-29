using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwards;

public record GetAwardsQuery(Guid TenantId, Guid UserId) : IRequest<ActionResult<AwardListResponse>>;
