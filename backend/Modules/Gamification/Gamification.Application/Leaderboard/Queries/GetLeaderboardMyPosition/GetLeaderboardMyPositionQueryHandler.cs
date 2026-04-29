using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboard;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboardMyPosition;

public class GetLeaderboardMyPositionQueryHandler : IRequestHandler<GetLeaderboardMyPositionQuery, ActionResult<MyLeaderboardPositionResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsTransactionRepository _transactionRepository;
    private readonly ILevelTierRepository _levelTierRepository;

    public GetLeaderboardMyPositionQueryHandler(
        IPlayerProfileRepository profileRepository,
        IPointsTransactionRepository transactionRepository,
        ILevelTierRepository levelTierRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _levelTierRepository = levelTierRepository;
    }

    public async Task<ActionResult<MyLeaderboardPositionResponse>> Handle(GetLeaderboardMyPositionQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult<MyLeaderboardPositionResponse>.Failure("Profile.NotFound");
        }

        var tier = await _levelTierRepository.GetForLevelAsync(request.TenantId, profile.CurrentLevel, ct);

        int periodXp;
        int position;

        if (request.Period == LeaderboardPeriod.AllTime)
        {
            periodXp = profile.TotalXp;
            position = await _profileRepository.GetRankByXpAsync(request.TenantId, profile.TotalXp, ct);
        }
        else
        {
            var from = request.Period == LeaderboardPeriod.Month
                ? DateTime.UtcNow.AddDays(-30)
                : DateTime.UtcNow.AddDays(-7);

            periodXp = await _transactionRepository.GetPeriodXpForProfileAsync(profile.Id, from, ct);
            var topRanking = await _transactionRepository.GetTopByPeriodAsync(request.TenantId, from, 10000, ct);
            position = topRanking.Count(r => r.PeriodXp > periodXp) + 1;
        }

        var response = new MyLeaderboardPositionResponse(
            position,
            profile.TotalXp,
            periodXp,
            profile.CurrentLevel,
            tier?.Title);

        return ActionResult<MyLeaderboardPositionResponse>.Success(response);
    }
}
