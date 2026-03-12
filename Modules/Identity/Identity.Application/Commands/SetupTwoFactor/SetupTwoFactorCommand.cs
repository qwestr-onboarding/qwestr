using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.SetupTwoFactor;

public record SetupTwoFactorCommand(Guid UserId) : IRequest<ActionResult<TwoFactorSetupResponse>>;
