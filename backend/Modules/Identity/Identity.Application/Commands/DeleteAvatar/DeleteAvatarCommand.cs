using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteAvatar;

public record DeleteAvatarCommand(Guid UserId) : IRequest<ActionResult>;
