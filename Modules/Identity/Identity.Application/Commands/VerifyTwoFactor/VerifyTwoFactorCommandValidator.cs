using FluentValidation;

namespace Qwestr.Modules.Identity.Application.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorCommandValidator()
    {
        RuleFor(x => x.TempToken).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
    }
}
