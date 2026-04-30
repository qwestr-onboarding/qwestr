using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Funnel;

public record GetScenarioFunnelQuery(Guid TenantId, Guid ScenarioId) : IRequest<ActionResult<ScenarioFunnelResponse>>;
