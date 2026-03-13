using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid TenantId, Guid TargetUserId) : IRequest<ActionResult<UserProfileResponse>>;
