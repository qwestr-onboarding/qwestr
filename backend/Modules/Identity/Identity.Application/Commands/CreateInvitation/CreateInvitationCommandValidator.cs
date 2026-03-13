using FluentValidation;
using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Application.Commands.CreateInvitation;

public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Role).NotEqual(UserRole.Owner).WithMessage("Cannot invite a user with the Owner role.");
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.InvitedByUserId).NotEmpty();
    }
}
