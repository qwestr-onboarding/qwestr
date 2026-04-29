using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.CreateAutomaticAward;

public record CreateAutomaticAwardCommand(
    Guid? TenantId,
    string Code,
    string Title,
    string Description,
    string IconUrl,
    RuleTrigger Trigger,
    RuleCondition Condition,
    int TargetValue,
    Dictionary<string, string>? FilterParams,
    int XpBonus,
    int CoinsBonus) : IRequest<ActionResult<Guid>>;
