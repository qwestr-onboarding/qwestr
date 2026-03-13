using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;

public record AcceptInvitationCommand(
    string Token,
    string Password,
    string FirstName,
    string LastName) : IRequest<ActionResult<TokenResponse>>;
