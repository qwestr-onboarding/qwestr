using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ConfirmTwoFactor;

public record ConfirmTwoFactorCommand(Guid UserId, string Code) : IRequest<ActionResult<BackupCodesResponse>>;
