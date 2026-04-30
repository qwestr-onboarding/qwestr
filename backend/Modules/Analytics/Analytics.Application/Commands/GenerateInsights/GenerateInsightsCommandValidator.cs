using FluentValidation;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Application.Commands.GenerateInsights;

public class GenerateInsightsCommandValidator : AbstractValidator<GenerateInsightsCommand>
{
    public GenerateInsightsCommandValidator()
    {
        RuleFor(x => x.Provider)
            .Must(p => Enum.IsDefined(typeof(LlmProvider), p))
            .WithMessage("analytics.insight.provider_invalid");
    }
}
