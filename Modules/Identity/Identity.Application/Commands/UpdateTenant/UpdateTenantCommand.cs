using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateTenant;

public record UpdateTenantCommand(Guid TenantId, string Name, string? Subdomain) : IRequest<ActionResult>;
