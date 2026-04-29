using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyAchievementProgress;

public class GetMyAchievementProgressQueryHandler : IRequestHandler<GetMyAchievementProgressQuery, ActionResult<AchievementProgressListResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IAwardRepository _awardRepository;

    public GetMyAchievementProgressQueryHandler(
        IPlayerProfileRepository profileRepository,
        IAwardRepository awardRepository)
    {
        _profileRepository = profileRepository;
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult<AchievementProgressListResponse>> Handle(GetMyAchievementProgressQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult<AchievementProgressListResponse>.Failure("Profile.NotFound");
        }

        var progressList = await _awardRepository.GetAllProgressByProfileAsync(profile.Id, ct);
        var allAwards = await _awardRepository.GetByTenantAsync(request.TenantId, ct);
        var awardDict = allAwards
            .Where(a => a.IssuanceType == AwardIssuanceType.Automatic)
            .ToDictionary(a => a.Id);

        var result = progressList
            .Where(p => awardDict.ContainsKey(p.AwardId))
            .Select(p =>
            {
                var award = awardDict[p.AwardId];
                return new AchievementProgressResponse(
                    p.AwardId,
                    award.Code,
                    award.Title,
                    award.IconUrl,
                    p.CurrentValue,
                    p.TargetValue,
                    p.IsCompleted,
                    p.CompletedAt);
            }).ToList();

        return ActionResult<AchievementProgressListResponse>.Success(new AchievementProgressListResponse(result));
    }
}
