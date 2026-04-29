using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Evaluators;

public class TestPassedWithFilterEvaluator : IRuleEvaluator
{
    public RuleTrigger Trigger => RuleTrigger.TestPassed;
    public RuleCondition Condition => RuleCondition.CountWithFilter;

    public Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext context, CancellationToken ct)
    {
        var filterParams = award.Rule?.FilterParams ?? new Dictionary<string, string>();

        if (filterParams.TryGetValue("minScore", out var minScoreStr)
            && int.TryParse(minScoreStr, out var minScore)
            && context.Payload.TryGetValue("score", out var scoreObj)
            && scoreObj is int score)
        {
            if (score < minScore)
            {
                return Task.FromResult(new EvaluationResult(0, null));
            }
        }

        return Task.FromResult(new EvaluationResult(1, null));
    }
}
