using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Evaluators;

public interface IRuleEvaluator
{
    RuleTrigger Trigger { get; }
    RuleCondition Condition { get; }
    Task<EvaluationResult> EvaluateAsync(Award award, EvaluationContext context, CancellationToken ct);
}
