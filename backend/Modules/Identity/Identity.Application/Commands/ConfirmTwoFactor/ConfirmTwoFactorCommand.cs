using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ConfirmTwoFactor;

public record ConfirmTwoFactorCommand(Guid UserId, string Code) : IRequest<ActionResult<BackupCodesResponse>>;
