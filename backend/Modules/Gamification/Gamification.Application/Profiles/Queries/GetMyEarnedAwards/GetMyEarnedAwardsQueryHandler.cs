using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyEarnedAwards;

public class GetMyEarnedAwardsQueryHandler : IRequestHandler<GetMyEarnedAwardsQuery, ActionResult<EarnedAwardListResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IAwardRepository _awardRepository;

    public GetMyEarnedAwardsQueryHandler(
        IPlayerProfileRepository profileRepository,
        IAwardRepository awardRepository)
    {
        _profileRepository = profileRepository;
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult<EarnedAwardListResponse>> Handle(GetMyEarnedAwardsQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult<EarnedAwardListResponse>.Failure("Profile.NotFound");
        }

        var earnedAwards = await _awardRepository.GetEarnedByProfileAsync(profile.Id, ct);
        var allAwards = await _awardRepository.GetByTenantAsync(request.TenantId, ct);
        var awardDict = allAwards.ToDictionary(a => a.Id);

        var result = earnedAwards
            .Where(e => awardDict.ContainsKey(e.AwardId))
            .Select(e =>
            {
                var award = awardDict[e.AwardId];
                return new EarnedAwardResponse(
                    e.Id,
                    e.AwardId,
                    award.Code,
                    award.Title,
                    award.IconUrl,
                    e.EarnedAt,
                    award.IssuanceType == AwardIssuanceType.Manual);
            }).ToList();

        return ActionResult<EarnedAwardListResponse>.Success(new EarnedAwardListResponse(result));
    }
}
