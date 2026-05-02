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
    private readonly IShopItemRepository _shopItemRepository;

    public GetLeaderboardQueryHandler(
        IPlayerProfileRepository profileRepository,
        IPointsTransactionRepository transactionRepository,
        ILevelTierRepository levelTierRepository,
        IUserProvider userProvider,
        IShopItemRepository shopItemRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
        _levelTierRepository = levelTierRepository;
        _userProvider = userProvider;
        _shopItemRepository = shopItemRepository;
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

        var frameItemIds = allProfiles
            .Where(p => p.Equipment.AvatarFrameItemId.HasValue)
            .Select(p => p.Equipment.AvatarFrameItemId!.Value)
            .Distinct();
        var frameItems = await _shopItemRepository.GetByIdsAsync(frameItemIds, ct);
        var frameIconDict = frameItems.ToDictionary(i => i.Id, i => i.IconUrl);
        var profileFrameDict = allProfiles
            .Where(p => p.Equipment.AvatarFrameItemId.HasValue
                        && frameIconDict.ContainsKey(p.Equipment.AvatarFrameItemId!.Value))
            .ToDictionary(p => p.Id, p => frameIconDict[p.Equipment.AvatarFrameItemId!.Value]);

        var result = ranked.Select((r, index) =>
        {
            var profile = profilesByIdDict.GetValueOrDefault(r.ProfileId);
            var userId = profile?.UserId ?? Guid.Empty;
            var user = userDict.GetValueOrDefault(userId);
            var displayName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
            var level = profile?.CurrentLevel ?? 1;
            var tier = tiers.FirstOrDefault(t => level >= t.FromLevel && level <= t.ToLevel);
            var avatarUrl = user?.AvatarUrl;
            var frameUrl = profile != null && profileFrameDict.TryGetValue(profile.Id, out var fu) ? fu : null;

            return new LeaderboardEntryResponse(
                index + 1,
                userId,
                displayName,
                level,
                tier?.Title,
                tier?.Color,
                r.TotalXp,
                r.PeriodXp,
                avatarUrl,
                frameUrl);
        }).ToList();

        return ActionResult<LeaderboardListResponse>.Success(new LeaderboardListResponse(result));
    }
}
