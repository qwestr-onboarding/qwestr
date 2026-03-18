using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateStep;

public class CreateStepCommandValidator : AbstractValidator<CreateStepCommand>
{
    public CreateStepCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.StageId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Points).GreaterThanOrEqualTo(0).When(x => x.Points.HasValue);
    }
}
