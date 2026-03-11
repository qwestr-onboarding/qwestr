using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string Subdomain) : IRequest<ActionResult<TokenResponse>>;
