using Identity.Application.DTOs;
using MediatR;
using Shared.Kernel.Entities;

namespace Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string Subdomain) : IRequest<ActionResult<TokenResponse>>;
