using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Dashboard;

public record GetTenantDashboardQuery(Guid TenantId) : IRequest<ActionResult<DashboardResponse>>;
