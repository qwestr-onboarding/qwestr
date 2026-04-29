using MediatR;
using Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboard;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboardMyPosition;

public record GetLeaderboardMyPositionQuery(Guid TenantId, Guid UserId, LeaderboardPeriod Period) : IRequest<ActionResult<MyLeaderboardPositionResponse>>;
