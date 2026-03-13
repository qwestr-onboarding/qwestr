using FluentValidation;

namespace Qwestr.Modules.Identity.Application.Commands.AcceptInvitation;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}
