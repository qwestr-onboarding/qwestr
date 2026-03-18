using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateQuestionOption;

public class CreateQuestionOptionCommandValidator : AbstractValidator<CreateQuestionOptionCommand>
{
    public CreateQuestionOptionCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
    }
}
