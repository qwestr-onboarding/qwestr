using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<ActionResult>;
