using FluentValidation;

namespace Qwestr.Modules.Identity.Application.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from the current password.");
    }
}
