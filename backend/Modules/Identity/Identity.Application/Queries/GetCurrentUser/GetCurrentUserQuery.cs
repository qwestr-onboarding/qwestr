using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<ActionResult<UserProfileResponse>>;
