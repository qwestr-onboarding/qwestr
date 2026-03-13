using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.SetupTwoFactor;

public record SetupTwoFactorCommand(Guid UserId) : IRequest<ActionResult<TwoFactorSetupResponse>>;
