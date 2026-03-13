using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.RequestPasswordReset;

public record RequestPasswordResetCommand(string Email, string Subdomain) : IRequest<ActionResult>;
