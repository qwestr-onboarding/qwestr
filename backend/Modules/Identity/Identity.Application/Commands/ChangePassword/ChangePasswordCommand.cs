using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<ActionResult>;
