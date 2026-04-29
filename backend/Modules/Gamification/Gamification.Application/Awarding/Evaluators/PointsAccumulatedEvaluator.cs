using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Evaluators;

public class PointsAccumulatedEvaluator : IRuleEvaluator
{
    public RuleTrigger Trigger => RuleTrigger.PointsAccumulated;
    public RuleCondition Condition => RuleCondition.Threshold;

    public Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext context, CancellationToken ct)
    {
        if (!context.Payload.TryGetValue("totalXp", out var xpObj) || xpObj is not int totalXp)
        {
            return Task.FromResult(new EvaluationResult(0, null));
        }
        return Task.FromResult(new EvaluationResult(0, totalXp));
    }
}
