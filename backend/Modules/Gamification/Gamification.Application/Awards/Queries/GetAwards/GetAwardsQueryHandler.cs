using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwards;

public class GetAwardsQueryHandler : IRequestHandler<GetAwardsQuery, ActionResult<AwardListResponse>>
{
    private readonly IAwardRepository _awardRepository;
    private readonly IPlayerProfileRepository _profileRepository;

    public GetAwardsQueryHandler(IAwardRepository awardRepository, IPlayerProfileRepository profileRepository)
    {
        _awardRepository = awardRepository;
        _profileRepository = profileRepository;
    }

    public async Task<ActionResult<AwardListResponse>> Handle(GetAwardsQuery request, CancellationToken ct)
    {
        var awards = await _awardRepository.GetByTenantAsync(request.TenantId, ct);

        var result = awards
            .Where(a => a.IsActive)
            .Select(a => MapToResponse(a))
            .ToList();

        return ActionResult<AwardListResponse>.Success(new AwardListResponse(result));
    }

    private static AwardResponse MapToResponse(Domain.Entities.Award a)
    {
        AwardRuleResponse? rule = null;
        if (a.Rule != null)
        {
            rule = new AwardRuleResponse(
                a.Rule.Trigger.ToString(),
                a.Rule.Condition.ToString(),
                a.Rule.TargetValue,
                new Dictionary<string, string>(a.Rule.FilterParams));
        }

        return new AwardResponse(
            a.Id, a.TenantId, a.Code, a.Title, a.Description, a.IconUrl,
            a.IssuanceType.ToString(), rule, a.XpBonus, a.CoinsBonus, a.IsActive);
    }
}
