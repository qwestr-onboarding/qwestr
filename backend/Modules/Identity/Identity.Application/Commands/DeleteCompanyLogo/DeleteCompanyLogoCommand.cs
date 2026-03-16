using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteCompanyLogo;

public record DeleteCompanyLogoCommand(Guid TenantId) : IRequest<ActionResult>;