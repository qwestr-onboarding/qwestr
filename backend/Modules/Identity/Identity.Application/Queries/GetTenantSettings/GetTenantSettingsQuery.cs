using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenantSettings;

public record GetTenantSettingsQuery(Guid TenantId) : IRequest<ActionResult<TenantSettingsResponse>>;
