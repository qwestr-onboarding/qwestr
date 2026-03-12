using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;

public record AcceptInvitationCommand(
    string Token,
    string Password,
    string FirstName,
    string LastName) : IRequest<ActionResult<TokenResponse>>;
