using MediatR;
using Shared.Kernel.Entities;

namespace Identity.Application.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ActionResult>;
