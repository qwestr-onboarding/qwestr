using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateTenantSettings;

public record UpdateTenantSettingsCommand(
    Guid TenantId,
    string? Timezone,
    string? Language) : IRequest<ActionResult>;
