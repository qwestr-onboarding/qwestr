using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Queries.GetAwardsForManagement;

public class GetAwardsForManagementQueryHandler : IRequestHandler<GetAwardsForManagementQuery, ActionResult<AwardListResponse>>
{
    private readonly IAwardRepository _awardRepository;

    public GetAwardsForManagementQueryHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult<AwardListResponse>> Handle(GetAwardsForManagementQuery request, CancellationToken ct)
    {
        var awards = await _awardRepository.GetByTenantAsync(request.TenantId, ct);

        var result = awards.Select(a =>
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
        }).ToList();

        return ActionResult<AwardListResponse>.Success(new AwardListResponse(result));
    }
}
