using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Logout;

public record LogoutCommand : IRequest<ActionResult>;
