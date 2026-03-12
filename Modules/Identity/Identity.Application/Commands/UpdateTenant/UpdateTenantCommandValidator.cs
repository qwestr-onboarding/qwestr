using FluentValidation;

namespace Qwestr.Modules.Identity.Application.Commands.UpdateTenant;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Subdomain)
            .MaximumLength(63)
            .Matches(@"^[a-z0-9]([a-z0-9-]*[a-z0-9])?$")
            .WithMessage("Subdomain must contain only lowercase letters, digits, and hyphens.")
            .When(x => x.Subdomain is not null);
    }
}
