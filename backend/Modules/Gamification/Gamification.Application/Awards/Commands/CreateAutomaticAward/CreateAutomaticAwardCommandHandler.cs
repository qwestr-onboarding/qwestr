using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.ValueObjects;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.CreateAutomaticAward;

public class CreateAutomaticAwardCommandHandler : IRequestHandler<CreateAutomaticAwardCommand, ActionResult<Guid>>
{
    private readonly IAwardRepository _awardRepository;

    public CreateAutomaticAwardCommandHandler(IAwardRepository awardRepository)
    {
        _awardRepository = awardRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateAutomaticAwardCommand request, CancellationToken ct)
    {
        var ruleResult = AwardRule.Create(
            request.Trigger,
            request.Condition,
            request.TargetValue,
            request.FilterParams);
        if (ruleResult.IsFailure)
        {
            return ActionResult<Guid>.Failure(ruleResult.Error);
        }

        var awardResult = Award.CreateAutomatic(
            request.TenantId,
            request.Code,
            request.Title,
            request.Description,
            request.IconUrl,
            ruleResult.Value!,
            request.XpBonus,
            request.CoinsBonus);
        if (awardResult.IsFailure)
        {
            return ActionResult<Guid>.Failure(awardResult.Error);
        }

        await _awardRepository.AddAsync(awardResult.Value!, ct);
        await _awardRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(awardResult.Value!.Id);
    }
}
