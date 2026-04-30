using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Insights;

public record GetCooldownStatusQuery(Guid TenantId) : IRequest<ActionResult<CooldownStatusResponse>>;
