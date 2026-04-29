using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Awarding.Models;

public record EvaluationContext(
    Guid TenantId,
    Guid ProfileId,
    RuleTrigger Trigger,
    Dictionary<string, object> Payload);
