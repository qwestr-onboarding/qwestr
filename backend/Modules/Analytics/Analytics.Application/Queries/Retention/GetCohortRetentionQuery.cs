using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Retention;

public record GetCohortRetentionQuery(Guid TenantId, int Weeks = 8)
    : IRequest<ActionResult<CohortRetentionResponse>>;
