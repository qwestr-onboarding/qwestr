using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DisableTwoFactor;

public record DisableTwoFactorCommand(Guid UserId, string Code) : IRequest<ActionResult>;
