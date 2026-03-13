using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateProfile;

public record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName) : IRequest<ActionResult>;
