using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Evaluators;

public class ScenarioCompletedCountEvaluator : IRuleEvaluator
{
    public RuleTrigger Trigger => RuleTrigger.ScenarioCompleted;
    public RuleCondition Condition => RuleCondition.Count;

    public Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext context, CancellationToken ct)
        => Task.FromResult(new EvaluationResult(1, null));
}
