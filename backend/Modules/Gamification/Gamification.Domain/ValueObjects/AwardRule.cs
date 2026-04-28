using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.ValueObjects;

public sealed class AwardRule
{
    public RuleTrigger Trigger { get; }
    public RuleCondition Condition { get; }
    public int TargetValue { get; }
    public IReadOnlyDictionary<string, string> FilterParams { get; }

    private AwardRule(
        RuleTrigger trigger,
        RuleCondition condition,
        int targetValue,
        IReadOnlyDictionary<string, string> filterParams)
    {
        Trigger = trigger;
        Condition = condition;
        TargetValue = targetValue;
        FilterParams = filterParams;
    }

    public static ActionResult<AwardRule> Create(
        RuleTrigger trigger,
        RuleCondition condition,
        int targetValue,
        IReadOnlyDictionary<string, string>? filterParams = null)
    {
        if (targetValue <= 0)
        {
            return ActionResult<AwardRule>.Failure("AwardRule.InvalidTargetValue");
        }
        if (condition == RuleCondition.CountWithFilter && (filterParams == null || filterParams.Count == 0))
        {
            return ActionResult<AwardRule>.Failure("AwardRule.MissingFilter");
        }
        return ActionResult<AwardRule>.Success(new AwardRule(trigger, condition, targetValue, filterParams ?? new Dictionary<string, string>()));
    }
}