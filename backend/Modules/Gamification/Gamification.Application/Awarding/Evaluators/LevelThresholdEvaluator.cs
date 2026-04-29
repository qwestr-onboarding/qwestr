using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Evaluators;

public class LevelThresholdEvaluator : IRuleEvaluator
{
    public RuleTrigger Trigger => RuleTrigger.LevelReached;
    public RuleCondition Condition => RuleCondition.Threshold;

    public Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext context, CancellationToken ct)
    {
        if (!context.Payload.TryGetValue("newLevel", out var levelObj) || levelObj is not int level)
        {
            return Task.FromResult(new EvaluationResult(0, null));
        }
        return Task.FromResult(new EvaluationResult(0, level));
    }
}
