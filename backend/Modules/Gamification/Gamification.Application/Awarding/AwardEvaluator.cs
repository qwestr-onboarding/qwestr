using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Modules.Gamification.Application.Awarding.Evaluators;
using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Application.Points;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding;

public class AwardEvaluator : IAwardEvaluator
{
    private readonly IAwardRepository _awardRepository;
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsService _pointsService;
    private readonly IPublisher _publisher;
    private readonly IEnumerable<IRuleEvaluator> _evaluators;

    public AwardEvaluator(
        IAwardRepository awardRepository,
        IPlayerProfileRepository profileRepository,
        IPointsService pointsService,
        IPublisher publisher,
        IEnumerable<IRuleEvaluator> evaluators)
    {
        _awardRepository = awardRepository;
        _profileRepository = profileRepository;
        _pointsService = pointsService;
        _publisher = publisher;
        _evaluators = evaluators;
    }

    public async Task<IReadOnlyList<EarnedAward>> EvaluateAsync(EvaluationContext context, CancellationToken ct)
    {
        var awards = await _awardRepository.GetActiveByTenantAndTriggerAsync(
            context.TenantId, AwardIssuanceType.Automatic, context.Trigger, ct);

        var profile = await _profileRepository.GetByIdAsync(context.ProfileId, ct);
        var earned = new List<EarnedAward>();

        foreach (var award in awards)
        {
            if (award.Rule == null)
            {
                continue;
            }

            var evaluator = _evaluators.FirstOrDefault(e =>
                e.Trigger == award.Rule.Trigger && e.Condition == award.Rule.Condition);
            if (evaluator == null)
            {
                continue;
            }

            var alreadyEarned = await _awardRepository.IsAwardAlreadyEarnedAsync(context.ProfileId, award.Id, ct);
            if (alreadyEarned)
            {
                continue;
            }

            var result = await evaluator.EvaluateAsync(award, context, ct);

            var progress = await _awardRepository.GetProgressAsync(context.ProfileId, award.Id, ct);
            if (progress == null)
            {
                progress = AchievementProgress.Create(context.TenantId, context.ProfileId, award.Id, award.Rule.TargetValue);
                await _awardRepository.AddProgressAsync(progress, ct);
            }

            bool completed;
            if (result.SetValue.HasValue)
            {
                completed = progress.SetValue(result.SetValue.Value);
            }
            else if (result.Delta > 0)
            {
                completed = progress.Increment(result.Delta);
            }
            else
            {
                continue;
            }

            if (completed)
            {
                var earnedAward = EarnedAward.CreateAutomatic(context.TenantId, context.ProfileId, award.Id);
                await _awardRepository.AddEarnedAwardAsync(earnedAward, ct);
                earned.Add(earnedAward);

                if ((award.XpBonus > 0 || award.CoinsBonus > 0) && profile != null)
                {
                    await _pointsService.AwardAsync(
                        context.TenantId,
                        context.ProfileId,
                        award.XpBonus,
                        award.CoinsBonus,
                        PointsSourceType.AwardBonus,
                        award.Id,
                        $"Получено достижение: {award.Title}",
                        ct);
                }

                await _publisher.Publish(
                    new AwardEarnedEvent(context.TenantId, context.ProfileId, profile?.UserId ?? Guid.Empty, award.Id, AwardIssuanceType.Automatic), ct);
                await _publisher.Publish(
                    new AwardEarnedIntegrationEvent(context.TenantId, profile?.UserId ?? Guid.Empty, award.Id, award.Code, award.Title, award.Description, earnedAward.EarnedAt), ct);
            }
        }

        await _awardRepository.SaveChangesAsync(ct);
        return earned;
    }
}
