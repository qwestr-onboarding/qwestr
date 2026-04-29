using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.ValueObjects;

public sealed class AwardRule
{
    public RuleTrigger Trigger { get; private set; }
    public RuleCondition Condition { get; private set; }
    public int TargetValue { get; private set; }
    public IReadOnlyDictionary<string, string> FilterParams { get; private set; } = new Dictionary<string, string>();

    private AwardRule() { }

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