using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Leaderboard.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, ActionResult<LeaderboardListResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsTransactionRepository _transactionRepository;
    private readonly ILevelTierRepository _levelTierRepository;
    private readonly IUserProvider _userProvider;

    public GetLeaderboardQueryHandler(
        IPlayerProfileRepository profileRepository,
        IPointsTransactionRepository transactionRepository,
        ILevelTierRepository levelTierRepository,
        IUserProvider userProvider)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _levelTierRepository = levelTierRepository;
        _userProvider = userProvider;
    }

    public async Task<ActionResult<LeaderboardListResponse>> Handle(GetLeaderboardQuery request, CancellationToken ct)
    {
        var tiers = await _levelTierRepository.GetByTenantAsync(request.TenantId, ct);
        var allProfiles = await _profileRepository.GetByTenantAsync(request.TenantId, ct);
        var profilesByIdDict = allProfiles.ToDictionary(p => p.Id);

        List<(Guid ProfileId, int TotalXp, int PeriodXp)> ranked;

        if (request.Period == LeaderboardPeriod.AllTime)
        {
            var topProfiles = await _profileRepository.GetTopByXpAsync(request.TenantId, request.TopN, ct);
            ranked = topProfiles.Select(p => (p.Id, p.TotalXp, p.TotalXp)).ToList();
        }
        else
        {
            var from = request.Period == LeaderboardPeriod.Month
                ? DateTime.UtcNow.AddDays(-30)
                : DateTime.UtcNow.AddDays(-7);

            var periodRanking = await _transactionRepository.GetTopByPeriodAsync(request.TenantId, from, request.TopN, ct);
            ranked = periodRanking
                .Where(r => profilesByIdDict.ContainsKey(r.ProfileId))
                .Select(r => (r.ProfileId, profilesByIdDict[r.ProfileId].TotalXp, r.PeriodXp))
                .ToList();
        }

        var userIds = ranked
            .Select(r => profilesByIdDict.GetValueOrDefault(r.ProfileId)?.UserId ?? Guid.Empty)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var users = await _userProvider.GetByIdsAsync(userIds, ct);
        var userDict = users.ToDictionary(u => u.Id);

        var result = ranked.Select((r, index) =>
        {
            var profile = profilesByIdDict.GetValueOrDefault(r.ProfileId);
            var userId = profile?.UserId ?? Guid.Empty;
            var user = userDict.GetValueOrDefault(userId);
            var displayName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
            var level = profile?.CurrentLevel ?? 1;
            var tier = tiers.FirstOrDefault(t => level >= t.FromLevel && level <= t.ToLevel);

            return new LeaderboardEntryResponse(
                index + 1,
                userId,
                displayName,
                level,
                tier?.Title,
                tier?.Color,
                r.TotalXp,
                r.PeriodXp);
        }).ToList();

        return ActionResult<LeaderboardListResponse>.Success(new LeaderboardListResponse(result));
    }
}
