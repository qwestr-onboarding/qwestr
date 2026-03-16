using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.UploadCompanyLogo;

public record UploadCompanyLogoCommand(Guid TenantId, Stream FileStream, long FileSize, string ContentType)
    : IRequest<ActionResult<string>>;