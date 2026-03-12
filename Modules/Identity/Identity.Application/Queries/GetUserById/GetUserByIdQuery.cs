using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid TenantId, Guid TargetUserId) : IRequest<ActionResult<UserProfileResponse>>;
