using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenantBySubdomain;

public record GetTenantBySubdomainQuery(string Subdomain) : IRequest<ActionResult<TenantPublicResponse>>;
