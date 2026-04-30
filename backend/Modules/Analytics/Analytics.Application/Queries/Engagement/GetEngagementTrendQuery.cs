using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Engagement;

public record GetEngagementTrendQuery(Guid TenantId, int Days = 30)
    : IRequest<ActionResult<EngagementTrendResponse>>;
