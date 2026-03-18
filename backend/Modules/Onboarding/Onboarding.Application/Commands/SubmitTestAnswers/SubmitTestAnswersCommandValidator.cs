using FluentValidation;

namespace Qwestr.Modules.Onboarding.Application.Commands.SubmitTestAnswers;

public class SubmitTestAnswersCommandValidator : AbstractValidator<SubmitTestAnswersCommand>
{
    public SubmitTestAnswersCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TestAttemptId).NotEmpty();
        RuleFor(x => x.Answers).NotNull().NotEmpty();
    }
}
