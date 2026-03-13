using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;

public record VerifyTwoFactorCommand(
    string TempToken,
    string Code,
    bool IsBackupCode) : IRequest<ActionResult<TokenResponse>>;
