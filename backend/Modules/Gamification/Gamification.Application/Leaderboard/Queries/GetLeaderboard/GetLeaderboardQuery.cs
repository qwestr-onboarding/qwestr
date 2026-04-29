using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboard;

public enum LeaderboardPeriod { AllTime, Month, Week }

public record GetLeaderboardQuery(Guid TenantId, LeaderboardPeriod Period, int TopN) : IRequest<ActionResult<LeaderboardListResponse>>;
