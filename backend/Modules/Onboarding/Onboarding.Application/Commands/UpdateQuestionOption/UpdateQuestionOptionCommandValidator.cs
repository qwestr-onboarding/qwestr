using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestionOption;

public class UpdateQuestionOptionCommandValidator : AbstractValidator<UpdateQuestionOptionCommand>
{
    public UpdateQuestionOptionCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.OptionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
    }
}
