using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwardsForManagement;

public record GetAwardsForManagementQuery(Guid TenantId) : IRequest<ActionResult<AwardListResponse>>;
