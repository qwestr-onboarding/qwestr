using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UploadAvatar;

public record UploadAvatarCommand(
    Guid UserId,
    Stream FileStream,
    long FileSize,
    string ContentType) : IRequest<ActionResult<string>>;
