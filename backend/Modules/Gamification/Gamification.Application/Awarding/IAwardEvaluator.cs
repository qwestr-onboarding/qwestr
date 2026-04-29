using Qwestr.Modules.Gamification.Application.Awarding.Models;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Application.Awarding;

public interface IAwardEvaluator
{
    Task<IReadOnlyList<EarnedAward>> EvaluateAsync(EvaluationContext context, CancellationToken ct);
}
