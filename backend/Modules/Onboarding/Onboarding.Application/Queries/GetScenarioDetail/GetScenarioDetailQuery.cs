using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetScenarioDetail;

public record GetScenarioDetailQuery(
    Guid TenantId,
    Guid ScenarioId) : IRequest<ActionResult<ScenarioDetailResponse>>;
