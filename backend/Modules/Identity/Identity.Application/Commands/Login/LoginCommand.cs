using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string Subdomain) : IRequest<ActionResult<TokenResponse>>;
