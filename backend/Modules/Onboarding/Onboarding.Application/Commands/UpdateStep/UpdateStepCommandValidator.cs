using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStep;

public class UpdateStepCommandValidator : AbstractValidator<UpdateStepCommand>
{
    public UpdateStepCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.StepId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Points).GreaterThanOrEqualTo(0).When(x => x.Points.HasValue);
    }
}
