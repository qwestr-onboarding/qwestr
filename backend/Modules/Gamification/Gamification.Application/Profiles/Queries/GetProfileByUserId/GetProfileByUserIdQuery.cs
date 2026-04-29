using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetProfileByUserId;

public record GetProfileByUserIdQuery(Guid TenantId, Guid UserId) : IRequest<ActionResult<PlayerProfileResponse>>;
