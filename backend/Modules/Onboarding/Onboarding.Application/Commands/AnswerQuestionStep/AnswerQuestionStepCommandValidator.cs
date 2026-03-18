using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.AnswerQuestionStep;

public class AnswerQuestionStepCommandValidator : AbstractValidator<AnswerQuestionStepCommand>
{
    public AnswerQuestionStepCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.StepProgressId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}
