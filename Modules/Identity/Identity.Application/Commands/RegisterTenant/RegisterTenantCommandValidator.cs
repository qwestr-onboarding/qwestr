using FluentValidation;

namespace Identity.Application.Commands.RegisterTenant;

public class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Subdomain)
            .Matches("^[a-z0-9-]+$").WithMessage("Subdomain may only contain lowercase letters, digits and hyphens.")
            .MaximumLength(63)
            .When(x => x.Subdomain is not null);

        RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.OwnerPassword).NotEmpty().MinimumLength(8);

        RuleFor(x => x.OwnerFirstName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.OwnerLastName).NotEmpty().MaximumLength(100);
    }
}
