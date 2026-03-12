using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetTenantSettings;

public record GetTenantSettingsQuery(Guid TenantId) : IRequest<ActionResult<TenantSettingsResponse>>;
