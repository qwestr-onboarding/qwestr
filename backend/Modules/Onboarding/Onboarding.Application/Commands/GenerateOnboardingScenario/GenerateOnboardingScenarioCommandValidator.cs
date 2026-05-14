using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.GenerateOnboardingScenario;

public class GenerateOnboardingScenarioCommandValidator : AbstractValidator<GenerateOnboardingScenarioCommand>
{
    private static readonly string[] ValidStyles = ["friendly", "strict", "simple"];

    public GenerateOnboardingScenarioCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Query).NotEmpty().MaximumLength(3000);
        RuleFor(x => x.StageCount).InclusiveBetween(1, 10);
        RuleFor(x => x.StepsPerStage).InclusiveBetween(1, 8);
        RuleFor(x => x.TextStyle).NotEmpty().Must(s => ValidStyles.Contains(s))
            .WithMessage("TextStyle must be one of: friendly, strict, simple");
        RuleFor(x => x.AdditionalInfo).MaximumLength(2000).When(x => x.AdditionalInfo != null);
        RuleFor(x => x.TargetAudience).MaximumLength(500).When(x => x.TargetAudience != null);
        RuleFor(x => x.TotalPoints).GreaterThan(0).When(x => x.TotalPoints.HasValue);
    }
}
