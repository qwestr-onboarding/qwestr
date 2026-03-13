using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;

public record VerifyTwoFactorCommand(
    string TempToken,
    string Code,
    bool IsBackupCode) : IRequest<ActionResult<TokenResponse>>;
