using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenant;

public record GetTenantQuery(Guid TenantId) : IRequest<ActionResult<TenantInfoResponse>>;
