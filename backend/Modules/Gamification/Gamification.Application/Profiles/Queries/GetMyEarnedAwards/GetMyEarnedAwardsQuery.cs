using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyEarnedAwards;

public record GetMyEarnedAwardsQuery(Guid TenantId, Guid UserId) : IRequest<ActionResult<EarnedAwardListResponse>>;
